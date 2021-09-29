using Lens.Core.Lib.Builders;
using Lens.Core.Lib.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lens.Core.Lib
{
    public static class ApplicationSetupBuilderExtensions
    {
        public static IApplicationSetupBuilder AddProgramInitializer<T>(this IApplicationSetupBuilder applicationSetup) where T : class, IProgramInitializer
        {
            applicationSetup.Services.AddTransient<IProgramInitializer, T>();
            return applicationSetup;
        }

        public static IApplicationSetupBuilder AddOAuthClient(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services
                .AddTransient<ApiBearerTokenHandler>()
                .Configure<OAuthClientSettings>(applicationSetup.Configuration.GetSection(nameof(OAuthClientSettings)))
                .AddScoped<IOAuthClientService, OAuthClientService>();

            return applicationSetup;
        }

        /// <summary>
        /// Add a HttpClient Service that gets a bearer-token from the configured IdentityServer
        /// </summary>
        public static IApplicationSetupBuilder AddHttpClientService<TClient, TImplementation>(this IApplicationSetupBuilder applicationSetup, string clientName, string baseUri = null)
            where TClient : class
            where TImplementation : class, TClient
        {
            applicationSetup.Services
                .AddHttpClient<TClient, TImplementation>()
                    .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseUri))
                    .AddHttpMessageHandler(services => {
                        var handler = services.GetRequiredService<ApiBearerTokenHandler>();
                        handler.ClientName = clientName;
                        return handler;
                    });

            return applicationSetup;
        }
    }
}
