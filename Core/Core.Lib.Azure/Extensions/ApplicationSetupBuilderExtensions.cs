using Azure.Identity;
using Lens.Core.Lib.Builders;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Azure.Extensions
{
    public static class ApplicationSetupBuilderExtensions
    {
        public static IApplicationSetupBuilder AddAzureApplicationsInsightsForWeb(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services.AddApplicationInsightsTelemetry(applicationSetup.Configuration);

            return applicationSetup;
        }

        public static IApplicationSetupBuilder AddAzureApplicationsInsightsForWorkers(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services.AddApplicationInsightsTelemetryWorkerService(applicationSetup.Configuration);

            return applicationSetup;
        }

        public static IApplicationSetupBuilder AddAzureClients(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services.AddAzureClients(builder => {
                var keyvaultName = applicationSetup.Configuration["KeyVault:Vault"];

                if (!string.IsNullOrEmpty(keyvaultName))
                {
                    builder.AddSecretClient(new Uri($"https://{keyvaultName}.vault.azure.net/"));
                }


                var blobStorageProvider = applicationSetup.Configuration["BlobSettings:Provider"];
                var blobStorageConnectionString = applicationSetup.Configuration["BlobSettings:ConnectionString"];

                if (!string.IsNullOrEmpty(blobStorageProvider) && blobStorageProvider.Equals("azure", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(blobStorageConnectionString))
                {
                    // blob storage will be added as singleton: https://docs.microsoft.com/en-us/dotnet/azure/sdk/thread-safety#client-lifetime
                    builder.AddBlobServiceClient(blobStorageConnectionString);
                }

                builder.UseCredential(new DefaultAzureCredential());

                builder.ConfigureDefaults(applicationSetup.Configuration.GetSection("AzureDefaults"));
            });

            return applicationSetup;
        }
    }
}
