using System;
using DioLive.Triangle.DataStorage;
using DioLive.Triangle.Protocol;
using DioLive.Triangle.Protocol.Json;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DioLive.Triangle.ServerCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RequestPool>();
            services.AddSingleton<Space>();
            services.AddSingleton<Random>();
            services.AddSingleton<ServerWorker>();
            services.AddSingleton<IProtocol, JsonProtocol>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();

            ServerWorker serverWorker = app.ApplicationServices.GetRequiredService<ServerWorker>();

            app.MapGet("/admin", cfg => cfg.Run(serverWorker.GetAdminAsync));
            app.MapPost("/create", cfg => cfg.Run(serverWorker.PostCreateAsync));
            app.MapGet("/current", cfg => cfg.Run(serverWorker.GetCurrentAsync));
            app.MapGet("/neighbours", cfg => cfg.Run(serverWorker.GetNeighboursAsync));
            app.MapGet("/radar", cfg => cfg.Run(serverWorker.GetRadarAsync));
            app.MapPost("/update", cfg => cfg.Run(serverWorker.PostUpdateAsync));
            app.MapPost("/signout", cfg => cfg.Run(serverWorker.PostSignoutAsync));

            serverWorker.StartAutoUpdate();
            serverWorker.UtilizeRequestPool();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}