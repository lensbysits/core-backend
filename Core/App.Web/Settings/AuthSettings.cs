namespace Lens.Core.App.Web
{
    public class AuthSettings
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public bool RequireHttps { get; set; } = true;
        public bool ValidateAudience { get; set; } = false;
        public string MetadataAddress { get; set; }
    }
}
