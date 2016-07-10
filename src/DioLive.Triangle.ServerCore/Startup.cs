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

            app.MapGet("/admin", cfg => cfg.Run(serverWorker.GetAdminAsync));
            app.MapPost("/create", cfg => cfg.Run(serverWorker.PostCreateAsync));
            app.MapGet("/current", cfg => cfg.Run(serverWorker.GetCurrentAsync));
            app.MapGet("/neighbours", cfg => cfg.Run(serverWorker.GetNeighboursAsync));
            app.MapGet("/radar", cfg => cfg.Run(serverWorker.GetRadarAsync));
            app.MapPost("/update", cfg => cfg.Run(serverWorker.PostUpdateAsync));
            app.MapPost("/signout", cfg => cfg.Run(serverWorker.PostSignoutAsync));

            serverWorker.StartAutoUpdate();
            //serverWorker.UtilizeRequestPool();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}