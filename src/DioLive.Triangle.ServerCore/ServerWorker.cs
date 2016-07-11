using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DataStorage;

using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

namespace DioLive.Triangle.ServerCore
{
    public class ServerWorker : IDisposable
    {
        private RequestPool requestPool;
        private Space space;
        private Random random;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private IHubContext mainHub;

        public ServerWorker(RequestPool requestPool, Space space, Random random)
        {
            this.requestPool = requestPool;
            this.space = space;
            this.random = random;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;

            this.mainHub = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
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
                        if (this.cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        DateTime now = DateTime.UtcNow;
                        this.space.Update(now - checkPoint);
                        checkPoint = now;

                        while (this.space.DestroyedDots.Count > 0)
                        {
                            Dot dot = this.space.DestroyedDots.Dequeue();
                            dynamic client = this.mainHub.Clients.Client(dot.Id.ToString());
                            client.OnDestroyed();
                        }

                        foreach (var dot in this.space.GetAllDots())
                        {
                            dynamic client = this.mainHub.Clients.Client(dot.Id.ToString());
                            client.OnUpdateCurrent(new CurrentResponse(dot.State, dot.MoveDirection, dot.BeamDirection));
                            client.OnUpdateNeighbours(new NeighboursResponse(this.space.GetNeighbours(dot.X, dot.Y).ToArray()));
                            client.OnUpdateRadar(new RadarResponse(this.space.GetRadar(dot.Team, dot.X, dot.Y).ToArray()));
                        }
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
                    while (!this.requestPool.IsCompleted)
                    {
                        UpdateRequest request = this.requestPool.Take();
                        Dot dot = this.space.FindById(request.Id);
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


        void IDisposable.Dispose()
        {
            StopAutoUpdate();
        }
    }
}