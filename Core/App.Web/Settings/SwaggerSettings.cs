using Lens.Core.Lib.Services;
using System.Collections.Generic;

namespace Lens.Core.App.Web
{
    public class SwaggerSettings : OAuthClientSetting
    {
        public SwaggerSettings()
        {
            this.ExtraDefinitions = new List<SwaggerDefinition>();
        }

        public string AppName { get; set; }
        public string ScopeName { get; set; }
        public string OpenAPIVersion { get; set; } = "3";
        public string ApiHostname { get; set; }
        public string XMLCommentsPath { get; set; }

        public List<SwaggerDefinition> ExtraDefinitions { get; set; }
    }

    public class SwaggerDefinition
    {
        public string AppName { get; set; }
        public string VersionInfo { get; set; }
        public string GroupName { get; set; }

    }
}
