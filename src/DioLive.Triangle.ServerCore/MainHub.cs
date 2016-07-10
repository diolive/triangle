using System;
using System.Threading.Tasks;
using Autofac;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DataStorage;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DioLive.Triangle.ServerCore
{
    [HubName("main")]
    public class MainHub : Hub
    {
        private ILifetimeScope lifetimeScope;
        private Random random;
        private Space space;

        public MainHub(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope.BeginLifetimeScope();
            this.random = lifetimeScope.Resolve<Random>();
            this.space = lifetimeScope.Resolve<Space>();
        }

        public override Task OnConnected()
        {
            Dot newDot = new Dot(Guid.Parse(Context.ConnectionId), (byte)this.random.Next(0, 3), 0, 0);
            this.space.Add(newDot);
            Clients.Caller.OnCreate(new CreateResponse(newDot.Id, newDot.Team));

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            this.space.RemoveById(Guid.Parse(Context.ConnectionId));

            return base.OnDisconnected(stopCalled);
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public void Update(UpdateRequest request)
        {
            Dot dot = this.space.FindById(request.Id);
            if (dot == null)
            {
                return;
            }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.lifetimeScope != null)
            {
                this.lifetimeScope.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}