using Autofac;
using Autofac.Integration.SignalR;

using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof(DioLive.Triangle.ServerCore.Startup))]

namespace DioLive.Triangle.ServerCore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = AutofacConfig.ConfigureContainer();
            app.UseAutofacMiddleware(container);
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);

            var scope = container.BeginLifetimeScope();

            ServerWorker serverWorker = scope.Resolve<ServerWorker>();

            app.MapSignalR();

            serverWorker.StartAutoUpdate();
            //serverWorker.UtilizeRequestPool();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}