using System;
using System.Diagnostics;
using Autofac;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Utilities;

namespace FlippinTen.Bootstrap
{
    public class AppContainer
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //ViewModels
            //builder.RegisterType<MenuViewModel>();

            //Services
            builder.RegisterType<OnlineCardGameService>().As<ICardGameOnlineService>();
            builder.RegisterType<OfflineCardGameService>().As<ICardGameOfflineService>();

            //Repositories
            builder.RegisterType<GenericRepository>().As<IGenericRepository>();

            //Utilities
            builder.RegisterType<CardGameUtilities>().As<ICardGameUtilities>();
            builder.RegisterType<CardUtilities>().As<ICardUtilities>();

            _container = builder.Build();
        }

        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }
    }
}
