using System;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;

using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;

using Domain.Abstract;
using Domain.Abstract.EntityRepositories;
using Domain.Entities;
using Domain.Concrete;
using Domain.Contexts;
using Domain.Concrete.Repositories;
using Domain.Providers;

namespace Domain.Helpers
{
    public static class NinjectResolver
    {
        public static StandardKernel kernel;

        public static Lazy<IKernel> CreateKernel = new Lazy<IKernel>(() =>
        {
            kernel = new StandardKernel();

            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
            RegisterServices(kernel);
            return kernel;
        });

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(KernelBase kernel)
        {
            kernel.Bind<IOAuthAuthorizationServerOptions>().To<MyOAuthAuthorizationServerOptions>();
            kernel.Bind<IOAuthAuthorizationServerProvider>().To<SimpleAuthorizationServerProvider>();
            kernel.Bind<IAuthenticationTokenProvider>().To<SimpleRefreshTokenProvider>();

            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            //
            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<IAuthRepository>().To<AuthRepository>();
            kernel.Bind<IBidRepository>().To<BidRepository>();
            kernel.Bind<ICityRepository>().To<CityRepository>();
            kernel.Bind<ITourRepository>().To<TourRepository>();
            //

        }
    }
}