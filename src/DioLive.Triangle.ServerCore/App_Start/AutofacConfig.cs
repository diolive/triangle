using System;
using Autofac;
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

            builder.RegisterType<RequestPool>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Space>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Random>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ServerWorker>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<BinaryProtocol>().As<IProtocol>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}