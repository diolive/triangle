using System.Collections.Generic;
using System.Text;
using DioLive.Triangle.BindingModels;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DioLive.Triangle.ServerCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RequestPool>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();

            app.MapGet("/state", GetState);
            app.MapPost("/update", PostUpdate);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private void GetState(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                var requestPool = context.RequestServices.GetRequiredService<RequestPool>();
                var updateRequests = new List<UpdateRequest>();
                while (requestPool.Count > 0)
                {
                    updateRequests.Add(requestPool.Take());
                }
                await context.Response.WriteAsync(JsonConvert.SerializeObject(updateRequests.ToArray()), Encoding.ASCII);
            });
        }

        private void PostUpdate(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                byte[] buffer = new byte[context.Request.ContentLength ?? 0];
                await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                string body = Encoding.ASCII.GetString(buffer);
                var updateRequest = JsonConvert.DeserializeObject<UpdateRequest>(body);
                var requestPool = context.RequestServices.GetRequiredService<RequestPool>();
                requestPool.Add(updateRequest);
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}