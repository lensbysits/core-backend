using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Azure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddAzureKeyVaultConfigProvider(this IConfigurationBuilder config, TokenCredential? credential = null, string configKeyName = "KeyVault:Vault")
        {
            var root = config.Build();

            if (credential is null)
            {
                credential = new DefaultAzureCredential();
            }

            // use managed identities in Azure Web Apps or user-secrets in development environments
            var keyvaultName = root[configKeyName];

            if (!string.IsNullOrEmpty(keyvaultName))
            {
                config.AddAzureKeyVault(new Uri($"https://{keyvaultName}.vault.azure.net/"), credential);
            }

            return config;
        }
    }
}
