using System;
using System.Net;
using System.Net.Http;

using System.Web.Http.Filters;
using Newtonsoft.Json;
using Domain.Exceptions;

namespace WebUI.Infrastructure
{
    public class ApplicationExceptionFilterAttribute : ExceptionFilterAttribute 
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is CustomAppException)
            {
                context.Response = new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new {message = context.Exception.Message })),
                    ReasonPhrase = "Bad Request",
                    StatusCode = HttpStatusCode.BadRequest
                };
               
            }
            else 
            {
                context.Response = new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new {message = Resources.Language.InternalError })),
                    ReasonPhrase = "Internal Server Error",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }
    }
}