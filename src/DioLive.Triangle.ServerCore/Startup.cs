using System;
using System.Linq;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DataStorage;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();

            app.MapPost("/create", PostCreate);
            app.MapGet("/state", GetState);
            app.MapPost("/update", PostUpdate);
            app.MapPost("/signout", PostSignout);

            Task.Run(() =>
            {
                var requestPool = app.ApplicationServices.GetRequiredService<RequestPool>();
                var space = app.ApplicationServices.GetRequiredService<Space>();

                Task.Run(async () =>
                {
                    DateTime checkPoint = DateTime.UtcNow;
                    while (true)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(40));
                        DateTime now = DateTime.UtcNow;
                        space.Update(now - checkPoint);
                        checkPoint = now;
                    }
                });

                while (!requestPool.IsCompleted)
                {
                    UpdateRequest request = requestPool.Take();
                    Dot dot = space.FindById(request.Id);
                    dot.MoveDirection = request.MoveDirection;
                    dot.Velocity = Space.InitVelocity;
                    dot.Beaming = request.Beaming;
                }
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private void PostCreate(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                var space = app.ApplicationServices.GetRequiredService<Space>();
                var random = app.ApplicationServices.GetRequiredService<Random>();

                Dot newDot = new Dot((byte)random.Next(0, 3), 0, 0);
                space.Add(newDot);
                await context.Response.WriteJsonAsync(new CreateDotResponse(newDot.Id, newDot.Team));
            });
        }

        private void GetState(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                var space = app.ApplicationServices.GetRequiredService<Space>();

                Guid id = Guid.Parse(context.Request.Query["Id"].First());
                Dot dot = space.FindById(id);
                if (dot == null)
                {
                    await context.Response.WriteJsonAsync(StateResponse.Destroyed);
                    return;
                }

                CurrentDot current = new CurrentDot((BindingModels.DotState)dot.State, dot.MoveDirection, dot.Beaming)
#if DEBUG
                { X = dot.X, Y = dot.Y }
#endif
                ;
                NeighbourDot[] neighbours = space.GetNeighbours((int)dot.X, (int)dot.Y)
                    .Select(d => new NeighbourDot(d.Team, (int)(d.X - dot.X), (int)(d.Y - dot.Y), d.State == DataStorage.DotState.Stunned, d.Beaming))
                    .ToArray();
                RadarDot[] radar = space.GetRadar(dot.Team, (int)dot.X, (int)dot.Y)
                    .Select(d => new RadarDot(d.Team, (int)(d.X - dot.X), (int)(d.Y - dot.Y)))
                    .ToArray();
                await context.Response.WriteJsonAsync(new StateResponse(current, neighbours, radar));
            });
        }

        private void PostUpdate(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                var updateRequest = await context.Request.ReadJsonAsync<UpdateRequest>();
                var requestPool = context.RequestServices.GetRequiredService<RequestPool>();
                requestPool.Add(updateRequest);
            });
        }

        private void PostSignout(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                var signoutRequest = await context.Request.ReadJsonAsync<SignoutRequest>();
                var space = context.RequestServices.GetRequiredService<Space>();
                space.RemoveById(signoutRequest.Id);
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}