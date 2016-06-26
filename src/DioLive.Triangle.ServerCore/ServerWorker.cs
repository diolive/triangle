using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DataStorage;
using DioLive.Triangle.Protocol;

using Microsoft.Owin;

namespace DioLive.Triangle.ServerCore
{
    public class ServerWorker : IDisposable
    {
        private RequestPool requestPool;
        private Space space;
        private Random random;

        private IProtocol protocol;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public ServerWorker(RequestPool requestPool, Space space, Random random, IProtocol protocol)
        {
            this.requestPool = requestPool;
            this.space = space;
            this.random = random;
            this.protocol = protocol;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
        }

        public void StartAutoUpdate()
        {
            Task.Run(
                async () =>
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
                },
                this.cancellationToken);
        }

        public void StopAutoUpdate()
        {
            this.cancellationTokenSource.Cancel();
        }

        public void UtilizeRequestPool()
        {
            Task.Run(
                () =>
                {
                    while (!requestPool.IsCompleted)
                    {
                        UpdateRequest request = requestPool.Take();
                        Dot dot = space.FindById(request.Id);
                        dot.MoveDirection = request.MoveDirection;
                        dot.Velocity = Space.InitVelocity;
                        if (request.BeamDirection.HasValue)
                        {
                            dot.BeamDirection = request.BeamDirection.Value;
                            dot.State |= DotState.Beaming;
                        }
                        else
                        {
                            dot.BeamDirection = default(byte);
                            dot.State &= ~DotState.Beaming;
                        }
                    }
                },
                this.cancellationToken);
        }

        // HACK: Debug use only
        public async Task GetAdminAsync(IOwinContext context)
        {
            var dots = this.space.GetAllDots().ToArray();
            await context.Response.WriteJsonAsync(dots);
        }

        public void GetAdmin(IOwinContext context)
        {
            GetAdminAsync(context).Wait();
        }

        public async Task PostCreateAsync(IOwinContext context)
        {
            Dot newDot = new Dot((byte)this.random.Next(0, 3), 0, 0);
            this.space.Add(newDot);
            var content = await protocol.CreateResponse.EncodeAsync(new CreateResponse(newDot.Id, newDot.Team));
            await content.CopyToAsync(context.Response.Body);
        }

        public void PostCreate(IOwinContext context)
        {
            PostCreateAsync(context).Wait();
        }

        public async Task GetCurrentAsync(IOwinContext context)
        {
            Guid id = Guid.Parse(context.Request.Query["Id"]);
            Dot dot = space.FindById(id);

            CurrentResponse response = (dot != null)
                ? new CurrentResponse(dot.State, dot.MoveDirection, dot.BeamDirection)
                : CurrentResponse.Destroyed;

            var content = await protocol.CurrentResponse.EncodeAsync(response);
            await content.CopyToAsync(context.Response.Body);
        }

        public void GetCurrent(IOwinContext context)
        {
            GetCurrentAsync(context).Wait();
        }

        public async Task GetNeighboursAsync(IOwinContext context)
        {
            Guid id = Guid.Parse(context.Request.Query["Id"]);
            Dot dot = space.FindById(id);

            NeighboursResponse response = (dot != null)
                ? new NeighboursResponse(space.GetNeighbours(dot.X, dot.Y).ToArray())
                : NeighboursResponse.None;

            var content = await protocol.NeighboursResponse.EncodeAsync(response);
            await content.CopyToAsync(context.Response.Body);
        }

        public void GetNeighbours(IOwinContext context)
        {
            GetNeighboursAsync(context).Wait();
        }

        public async Task GetRadarAsync(IOwinContext context)
        {
            Guid id = Guid.Parse(context.Request.Query["Id"]);
            Dot dot = space.FindById(id);

            RadarResponse response = (dot != null)
                ? new RadarResponse(space.GetRadar(dot.Team, dot.X, dot.Y).ToArray())
                : RadarResponse.None;

            var content = await protocol.RadarResponse.EncodeAsync(response);
            await content.CopyToAsync(context.Response.Body);
        }

        public void GetRadar(IOwinContext context)
        {
            GetRadarAsync(context).Wait();
        }

        public void PostUpdate(IOwinContext context)
        {
            var updateRequest = protocol.UpdateRequest.Read(context.Request.Body);
            requestPool.Add(updateRequest);
        }

        public async Task PostUpdateAsync(IOwinContext context)
        {
            await Task.Run(() => PostUpdate(context));
        }

        public void PostSignout(IOwinContext context)
        {
            var signoutRequest = protocol.SignoutRequest.Read(context.Request.Body);
            space.RemoveById(signoutRequest.Id);
        }

        public async Task PostSignoutAsync(IOwinContext context)
        {
            await Task.Run(() => PostSignout(context));
        }

        void IDisposable.Dispose()
        {
            this.StopAutoUpdate();
        }
    }
}