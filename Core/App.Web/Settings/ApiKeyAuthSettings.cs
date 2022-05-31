using Lens.Core.App.Web.Authentication;
using System;

namespace Lens.Core.App.Web
{
    public class ApiKeyAuthSettings : AuthSettings
    {
        public override string AuthenticationType => AuthenticationMethod.ApiKey;

        public string ApiKeyHeader { get; set; } = "X-Api-Key";

        public string ApiKey { get; set; }
    }
}
