using Lens.Services.Communication.Settings;
using Microsoft.Extensions.Options;

namespace Lens.Services.Communication.HttpHandlers;

public class SendSmsHttpHandler : DelegatingHandler
{
    private readonly SendSmsSettings _settings;

    public SendSmsHttpHandler(IOptions<SendSmsSettings> options)
    {
        _settings = options.Value;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = new UriBuilder(request.RequestUri!);
        uri.Query += $"&User={_settings.Username}&Password={_settings.Password}";
        request.RequestUri = uri.Uri;
        return base.SendAsync(request, cancellationToken);
    }
}
