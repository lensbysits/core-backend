using Lens.Core.Lib.Builders;
using Lens.Core.Lib.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

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
                    .AddHttpMessageHandler(services =>
                    {
                        var handler = services.GetRequiredService<ApiBearerTokenHandler>();
                        handler.ClientName = clientName;
                        return handler;
                    });

            return applicationSetup;
        }

        /// <summary>
        /// Add a HttpClient Service without getting a bearer-token from the configured IdentityServer
        /// This method should be only used for testing-purpose.
        /// </summary>
        public static IApplicationSetupBuilder AddHttpClientServiceWithoutToken<TClient, TImplementation>(this IApplicationSetupBuilder applicationSetup, string baseUri = null)
            where TClient : class
            where TImplementation : class, TClient
        {
            applicationSetup.Services
                .AddHttpClient<TClient, TImplementation>()
                    .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseUri));

            return applicationSetup;
        }

        /// <summary>
        /// Add a HttpClient Service that gets a bearer-token from the configured IdentityServer
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        public static IApplicationSetupBuilder AddHttpClientService<TClient, TImplementation>(this IApplicationSetupBuilder builder, Action<HttpClient> configureClient = null)
            where TClient : class
            where TImplementation : class, TClient
        {
            builder.AddHttpClientService<TClient, TImplementation, ApiBearerTokenHandler>(configureClient);

            return builder;
        }

        /// <summary>
        /// Add a HttpClient Service that gets a bearer-token from the configured IdentityServer
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        public static IApplicationSetupBuilder AddHttpClientService<TClient, TImplementation, THttpMessageHandler>(this IApplicationSetupBuilder builder, Action<HttpClient> configureClient = null)
            where TClient : class
            where TImplementation : class, TClient
            where THttpMessageHandler : DelegatingHandler
        {
            builder.Services
                .AddHttpClient<TClient, TImplementation>()
                    .ConfigureHttpClient(configureClient ?? (client => { }))
                    .AddHttpMessageHandler<THttpMessageHandler>();

            return builder;
        }

        public static IApplicationSetupBuilder AddBackgroundTaskQueue(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services
                .AddHostedService<QueuedHostedService>()
                .AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            return applicationSetup;
        }
    }
}
