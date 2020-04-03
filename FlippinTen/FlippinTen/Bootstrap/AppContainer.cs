using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autofac;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.ViewModels;

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
            builder.RegisterType<GameMenuService>().As<IGameMenuService>();
            builder.RegisterType<CardGameService>().As<ICardGameService>();

            //Repositories
            builder.RegisterType<GenericRepository>().As<IGenericRepository>();

            //builder.RegisterType<GamePlayService>().As<ICardGameEngine>();

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
