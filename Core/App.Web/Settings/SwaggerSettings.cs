using Lens.Core.Lib.Services;

namespace Lens.Core.App.Web;

public class SwaggerSettings : OAuthClientSetting
{
    public string? AppName { get; set; }
    public string? ScopeName { get; set; }
    public string OpenAPIVersion { get; set; } = "3";
    public string? ApiHostname { get; set; }
    public string? XMLCommentsPath { get; set; }
}
