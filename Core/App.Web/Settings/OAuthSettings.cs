using Lens.Core.App.Web.Authentication;
using System;

namespace Lens.Core.App.Web
{
    public class OAuthSettings : AuthSettings
    {
        public override string AuthenticationType => AuthenticationMethod.OAuth2;
        public string Authority { get; set; }
        public string Audience { get; set; }
        public bool RequireHttps { get; set; } = true;
        public bool ValidateAudience { get; set; } = false;
        public string MetadataAddress { get; set; }
    }
}
