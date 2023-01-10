using Lens.Core.Lib.Services;

namespace Lens.Core.App.Web;

public class SwaggerSettings : OAuthClientSetting
{
    public SwaggerSettings()
    {
        this.ExtraDefinitions = new List<SwaggerDefinition>();
    }

    public string? AppName { get; set; }
    public string? ScopeName { get; set; }
    public string OpenAPIVersion { get; set; } = "3";
    public string? ApiHostname { get; set; }
    public string? XMLCommentsPath { get; set; }

    public List<SwaggerDefinition> ExtraDefinitions { get; set; }

    public bool SwaggerAuthEnabled => !string.IsNullOrWhiteSpace(this.Authority)
                                        && !string.IsNullOrWhiteSpace(this.Scope)
                                        && !string.IsNullOrWhiteSpace(this.ScopeName);
}

public class SwaggerDefinition
{
    public string? AppName { get; set; }
    public string? VersionInfo { get; set; }
    public string? GroupName { get; set; }

}
