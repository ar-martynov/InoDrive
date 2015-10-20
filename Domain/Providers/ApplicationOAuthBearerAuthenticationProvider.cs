using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using Domain.Abstract;

namespace Domain.Providers
{
    public class ApplicationOAuthBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {

        /// <summary>
        /// Handles processing OAuth bearer token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            // try to find bearer token in a cookie 
            // (by default OAuthBearerAuthenticationHandler 
            // only checks Authorization header)

            //cookie auth for SignalR Websockets
            string tokenFromCookie = context.OwinContext.Request.Cookies["token"];

            if (context.Token == null && !string.IsNullOrEmpty(tokenFromCookie))
                context.Token = tokenFromCookie;
            return Task.FromResult<object>(null);
        }
    }
}
