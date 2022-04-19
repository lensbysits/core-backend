using System;

namespace Lens.Core.App.Web
{
    public class AuthSettings
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public bool RequireHttps { get; set; } = true;
        public bool ValidateAudience { get; set; } = false;
        public string MetadataAddress { get; set; }
        public string AuthenticationType { get; set; } = "oauth2";
        public string ApiKeyHeader { get; set; } = "X-Api-Key";
        public string ApiKey { get; set; }
        public string[] AllowedIssuers { get; set; } = Array.Empty<string>();
        public string[] RequiredScopes { get; set; } = Array.Empty<string>();
        public string[] RequiredAppRoles { get; set; } = Array.Empty<string>();
    }
}
