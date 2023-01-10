using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Lens.Services.Communication.Web.Client.Services;
using Lens.Services.Communication.Web.Client.Settings;
using Microsoft.Extensions.Configuration;

namespace Lens.Services.Communication.Web.Client;

public static class IApplicationSetupBuilderExtentions
{
    private static CommunicationSettings? _settings;

    public static IApplicationSetupBuilder AddCommunicationClient(this IApplicationSetupBuilder builder)
    {
        _settings = builder.Configuration.GetSection(nameof(CommunicationSettings))?.Get<CommunicationSettings>() ?? throw new Exception($"Missing settings for '{nameof(CommunicationSettings)}'");

        builder
            .AddHttpClientService<ICommunicationService, CommunicationService>(client => {
                client.BaseAddress = new Uri(_settings.Uri ?? throw new Exception($"Missing settings for '{nameof(CommunicationSettings.Uri)}'"));
            });

        return builder;
    }
}
