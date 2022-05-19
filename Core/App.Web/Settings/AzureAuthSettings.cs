using Lens.Core.App.Web.Authentication;
using System;

namespace Lens.Core.App.Web
{
    public class AzureAuthSettings : OAuthSettings
    {
        public override string AuthenticationType => AuthenticationMethod.AzureAd;

        public string[] AllowedIssuers { get; set; } = Array.Empty<string>();
        public string[] RequiredScopes { get; set; } = Array.Empty<string>();
        public string[] RequiredAppRoles { get; set; } = Array.Empty<string>();

        public bool IncludeConfigInBearerHeader { get; set; }
    }
}
