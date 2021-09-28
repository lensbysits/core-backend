using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using httpClient = System.Net.Http.HttpClient;

namespace CoreLib.Services
{
    public class OAuthClientService : IOAuthClientService
    {
        private readonly httpClient _httpClient;
        private readonly ILogger<OAuthClientService> _logger;
        private readonly OAuthClientSettings _oauthClientSettings;

        public OAuthClientService(
            httpClient httpClient,
            ILogger<OAuthClientService> logger,
            IOptionsSnapshot<OAuthClientSettings> authClientOptions)
        {
            _httpClient = httpClient;
            _logger = logger;
            _oauthClientSettings = authClientOptions.Value;
        }

        public async Task<string> GetBearerToken(string clientName)
        {
            if (!_oauthClientSettings.TryGetValue(clientName, out var clientSettings))
            {
                throw new Exception($"Settings are missing for client '{clientName}'. Please add {nameof(OAuthClientService)}:{clientName} to the settings.");
            }

            return await GetToken(clientSettings);
        }

        private async Task<string> GetToken(OAuthClientSetting clientSettings)
        {
            var discoDocument = new DiscoveryDocumentRequest
            {
                Address = clientSettings.Authority,
                Policy =
                {
                    Authority = clientSettings.Authority,
                    ValidateEndpoints = false,
                    RequireHttps = clientSettings.RequireHttps,
                }
            };

            var disco = await _httpClient.GetDiscoveryDocumentAsync(discoDocument);

            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
                return null;
            }

            // request token
            var response = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = clientSettings.ClientId,
                ClientSecret = clientSettings.ClientSecret,
                Scope = clientSettings.Scope
            });


            if (response.IsError)
            {
                _logger.LogError(response.Error);
                return null;
            }

            return response.AccessToken;
        }
    }
}
