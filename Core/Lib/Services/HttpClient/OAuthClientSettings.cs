namespace Lens.Core.Lib.Services;

public class OAuthClientSettings : Dictionary<string, OAuthClientSetting>
{
}
    
public class OAuthClientSetting
{
    public string? Authority { get; set; }
    public bool RequireHttps { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Scope { get; set; }
}
