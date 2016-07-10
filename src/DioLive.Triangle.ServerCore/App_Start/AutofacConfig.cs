using System;
using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using DioLive.Triangle.DataStorage;
using DioLive.Triangle.Protocol;
using DioLive.Triangle.Protocol.Binary;

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
            builder.RegisterType<BinaryProtocol>().As<IProtocol>().SingleInstance();

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            return builder.Build();
        }
    }
}