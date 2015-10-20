using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using Microsoft.AspNet.SignalR;


using Domain.Abstract.EntityRepositories;
using WebUI.Infrastructure;

using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;

[assembly: OwinStartup(typeof(WebUI.Startup))]
namespace WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
            GlobalHost.HubPipeline.RequireAuthentication();
            GlobalHost.HubPipeline.AddModule(new ApplicationHubPipelineModule());

            WebApiConfig.Register(config);
            AutomapperConfig.Initialize();
            

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseNinjectMiddleware(() => Domain.Helpers.NinjectResolver.CreateKernel.Value);
            app.UseNinjectWebApi(config);
        }
    }

   
}
