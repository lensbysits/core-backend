namespace Lens.Core.App.Web
{
    public class SwaggerSettings
    {
        public string AppName { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string ScopeName { get; set; }
        public string OpenAPIVersion { get; set; } = "3";
        public string ApiHostname { get; set; }
        public string XMLCommentsPath { get; set; }
    }
}
