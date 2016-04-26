using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DataStorage;
using Microsoft.AspNet.Http;

namespace DioLive.Triangle.ServerCore
{
    public class ServerWorker : IDisposable
    {
        private RequestPool requestPool;
        private Space space;
        private Random random;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public ServerWorker(RequestPool requestPool, Space space, Random random)
        {
            this.requestPool = requestPool;
            this.space = space;
            this.random = random;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
        }

        public void StartAutoUpdate()
        {
            Task.Run(async () =>
            {
                DateTime checkPoint = DateTime.UtcNow;
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(40));
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    DateTime now = DateTime.UtcNow;
                    space.Update(now - checkPoint);
                    checkPoint = now;
                }
            }, this.cancellationToken);
        }

        public void StopAutoUpdate()
        {
            this.cancellationTokenSource.Cancel();
        }

        public void UtilizeRequestPool()
        {
            Task.Run(() =>
            {
                while (!requestPool.IsCompleted)
                {
                    UpdateRequest request = requestPool.Take();
                    Dot dot = space.FindById(request.Id);
                    dot.MoveDirection = request.MoveDirection;
                    dot.Velocity = Space.InitVelocity;
                    dot.Beaming = request.Beaming;
                }
            }, this.cancellationToken);
        }

        public async Task PostCreate(HttpContext context)
        {
            Dot newDot = new Dot((byte)this.random.Next(0, 3), 0, 0);
            this.space.Add(newDot);
            await context.Response.WriteJsonAsync(new CreateDotResponse(newDot.Id, newDot.Team));
        }

        public async Task GetState(HttpContext context)
        {
            Guid id = Guid.Parse(context.Request.Query["Id"].First());
            Dot dot = space.FindById(id);
            if (dot == null)
            {
                await context.Response.WriteJsonAsync(StateResponse.Destroyed);
                return;
            }

            CurrentDot current = new CurrentDot((BindingModels.DotState)dot.State, dot.MoveDirection, dot.Beaming);
#if DEBUG
            current.X = dot.X;
            current.Y = dot.Y;
#endif

            NeighbourDot[] neighbours = space.GetNeighbours((int)dot.X, (int)dot.Y)
                .Select(d => new NeighbourDot(d.Team, (int)(d.X - dot.X), (int)(d.Y - dot.Y), d.State == DataStorage.DotState.Stunned, d.Beaming))
                .ToArray();
            RadarDot[] radar = space.GetRadar(dot.Team, (int)dot.X, (int)dot.Y)
                .Select(d => new RadarDot(d.Team, (int)(d.X - dot.X), (int)(d.Y - dot.Y)))
                .ToArray();
            await context.Response.WriteJsonAsync(new StateResponse(current, neighbours, radar));
        }

        public async Task PostUpdate(HttpContext context)
        {
            var updateRequest = await context.Request.ReadJsonAsync<UpdateRequest>();
            requestPool.Add(updateRequest);
        }

        public async Task PostSignout(HttpContext context)
        {
            var signoutRequest = await context.Request.ReadJsonAsync<SignoutRequest>();
            space.RemoveById(signoutRequest.Id);
        }

        void IDisposable.Dispose()
        {
            this.StopAutoUpdate();
        }
    }
}