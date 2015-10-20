using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

using Newtonsoft.Json.Serialization;

using WebUI.Infrastructure;

namespace WebUI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // http://stackoverflow.com/questions/22157596/asp-net-web-api-operationcanceledexception-when-browser-cancels-the-request
            config.MessageHandlers.Add(new WebUI.Infrastructure.CancelledTaskBugWorkaroundMessageHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();
            

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { controller = @"[^\.]*" }
            );

            config.MessageHandlers.Add(new LanguageMessageHandler());
            
            //Elmah error handler
            //http://www.jasonwatmore.com/post/2014/05/03/Getting-ELMAH-to-catch-ALL-unhandled-exceptions-in-Web-API-21.aspx
            //config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            config.Filters.Add(new Elmah.Contrib.WebApi.ElmahHandleErrorApiAttribute());
            config.Routes.IgnoreRoute("Axd", "{whatever}.axd/{*pathInfo}");

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
