using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DataStorage;

using Microsoft.Owin;
using Microsoft.AspNet.SignalR;

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

        #region Protocol usage
        ////public async Task PostCreateAsync(IOwinContext context)
        ////{
        ////    Dot newDot = new Dot((byte)this.random.Next(0, 3), 0, 0);
        ////    this.space.Add(newDot);
        ////    var content = await this.protocol.CreateResponse.EncodeAsync(new CreateResponse(newDot.Id, newDot.Team));
        ////    await content.CopyToAsync(context.Response.Body);
        ////}

        ////public void PostCreate(IOwinContext context)
        ////{
        ////    PostCreateAsync(context).Wait();
        ////}

        ////public async Task GetCurrentAsync(IOwinContext context)
        ////{
        ////    Guid id = Guid.Parse(context.Request.Query["Id"]);
        ////    Dot dot = this.space.FindById(id);

        ////    CurrentResponse response = (dot != null)
        ////        ? new CurrentResponse(dot.State, dot.MoveDirection, dot.BeamDirection)
        ////        : CurrentResponse.Destroyed;

        ////    var content = await this.protocol.CurrentResponse.EncodeAsync(response);
        ////    await content.CopyToAsync(context.Response.Body);
        ////}

        ////public void GetCurrent(IOwinContext context)
        ////{
        ////    GetCurrentAsync(context).Wait();
        ////}

        ////public async Task GetNeighboursAsync(IOwinContext context)
        ////{
        ////    Guid id = Guid.Parse(context.Request.Query["Id"]);
        ////    Dot dot = this.space.FindById(id);

        ////    NeighboursResponse response = (dot != null)
        ////        ? new NeighboursResponse(this.space.GetNeighbours(dot.X, dot.Y).ToArray())
        ////        : NeighboursResponse.None;

        ////    var content = await this.protocol.NeighboursResponse.EncodeAsync(response);
        ////    await content.CopyToAsync(context.Response.Body);
        ////}

        ////public void GetNeighbours(IOwinContext context)
        ////{
        ////    GetNeighboursAsync(context).Wait();
        ////}

        ////public async Task GetRadarAsync(IOwinContext context)
        ////{
        ////    Guid id = Guid.Parse(context.Request.Query["Id"]);
        ////    Dot dot = this.space.FindById(id);

        ////    RadarResponse response = (dot != null)
        ////        ? new RadarResponse(this.space.GetRadar(dot.Team, dot.X, dot.Y).ToArray())
        ////        : RadarResponse.None;

        ////    var content = await this.protocol.RadarResponse.EncodeAsync(response);
        ////    await content.CopyToAsync(context.Response.Body);
        ////}

        ////public void GetRadar(IOwinContext context)
        ////{
        ////    GetRadarAsync(context).Wait();
        ////}

        ////public void PostUpdate(IOwinContext context)
        ////{
        ////    var updateRequest = this.protocol.UpdateRequest.Read(context.Request.Body);
        ////    this.requestPool.Add(updateRequest);
        ////}

        ////public async Task PostUpdateAsync(IOwinContext context)
        ////{
        ////    await Task.Run(() => PostUpdate(context));
        ////}

        ////public void PostSignout(IOwinContext context)
        ////{
        ////    var signoutRequest = this.protocol.SignoutRequest.Read(context.Request.Body);
        ////    this.space.RemoveById(signoutRequest.Id);
        ////}

        ////public async Task PostSignoutAsync(IOwinContext context)
        ////{
        ////    await Task.Run(() => PostSignout(context));
        ////} 
        #endregion

        void IDisposable.Dispose()
        {
            StopAutoUpdate();
        }
    }
}