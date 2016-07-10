using System;
using System.Reflection;

using Autofac;
using Autofac.Integration.SignalR;

using DioLive.Triangle.DataStorage;

namespace DioLive.Triangle.ServerCore
{
    public class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RequestPool>().AsSelf().SingleInstance();
            builder.RegisterType<Space>().AsSelf().SingleInstance();
            builder.RegisterType<Random>().AsSelf().SingleInstance();
            builder.RegisterType<ServerWorker>().AsSelf().SingleInstance();

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            return builder.Build();
        }
    }
}