using Lens.Core.Lib.Builders;
using Microsoft.Extensions.Configuration;
using Lens.Core.Lib.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Blob
{
    public static class BlobServiceBuilderExtensions
    {
        public static IApplicationSetupBuilder AddBlobService(this IApplicationSetupBuilder builder, BlobImplementationType blobImplementationType)
        {
            var blobSettings = builder.Configuration.GetSection(nameof(BlobSettings)).Get<BlobSettings>();
            if (blobSettings == null)
            {
                throw new NotFoundException($"Missing '{nameof(BlobSettings)}' configuration section.");
            }
            if (string.IsNullOrEmpty(blobSettings.ContainerName))
            {
                throw new NotFoundException($"Missing value for '{nameof(BlobSettings.ContainerName)}'");
            }
            if (string.IsNullOrEmpty(blobSettings.ConnectionString) && blobImplementationType == BlobImplementationType.AzureStorage)
            {
                throw new NotFoundException($"Missing value for '{nameof(BlobSettings.ConnectionString)}'");
            }

            _ = blobImplementationType == BlobImplementationType.Filesystem
                ? builder.Services.AddScoped<IBlobService, FilesystemBlobService>()
                : blobImplementationType == BlobImplementationType.AzureStorage
                ? builder.Services.AddScoped<IBlobService, AzureStorageBlobService>()
                : throw new NotFoundException("Unknown blob implementation type");

            return builder;
        }
    }
}
