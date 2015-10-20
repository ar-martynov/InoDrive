using Microsoft.Owin.Security.OAuth;

namespace Domain.Abstract
{
        public interface IOAuthAuthorizationServerOptions
        {
            OAuthAuthorizationServerOptions GetOptions();
        };
}
