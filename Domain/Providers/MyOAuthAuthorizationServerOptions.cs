using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using Domain.Abstract;

namespace Domain.Providers
{
    /// <summary>
    /// This class implement DI for RefreshTokenProvider and AuthServerProvider
    /// </summary>
    public class MyOAuthAuthorizationServerOptions:IOAuthAuthorizationServerOptions
    {
        private IOAuthAuthorizationServerProvider _provider;
        private IAuthenticationTokenProvider _tokenProvider;

        public MyOAuthAuthorizationServerOptions(IAuthenticationTokenProvider tProvider,
                                                 IOAuthAuthorizationServerProvider provider)
        {
            _provider = provider;
            _tokenProvider = tProvider;
        }

        public OAuthAuthorizationServerOptions GetOptions()
        {
            return new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = _provider,
                RefreshTokenProvider = _tokenProvider
            };
        }
    }
}