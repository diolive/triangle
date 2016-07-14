using System;
using System.Collections.Generic;
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
        private Dictionary<Guid, dynamic> clients;

        public ServerWorker(RequestPool requestPool, Space space, Random random)
        {
            this.requestPool = requestPool;
            this.space = space;
            this.random = random;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;

            this.mainHub = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
            this.clients = new Dictionary<Guid, dynamic>();
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
                            dynamic client = GetClient(dot.Id);
                            client.OnDestroyed();
                            this.clients.Remove(dot.Id);
                        }

                        foreach (var dot in this.space.GetAllDots())
                        {
                            dynamic client = GetClient(dot.Id);
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
                        this.space.ProcessUpdateRequest(this.requestPool.Take());
                    }
                },
                this.cancellationToken);
        }

        private dynamic GetClient(Guid id)
        {
            if (!this.clients.ContainsKey(id))
            {
                this.clients[id] = this.mainHub.Clients.Client(id.ToString());
            }

            return this.clients[id];
        }

        void IDisposable.Dispose()
        {
            StopAutoUpdate();
        }
    }
}