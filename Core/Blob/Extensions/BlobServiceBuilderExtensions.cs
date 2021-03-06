using Lens.Core.Lib.Builders;
using Microsoft.Extensions.Configuration;
using Lens.Core.Lib.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Lens.Core.Lib;
using Lens.Core.Blob.Data;
using Lens.Core.Data.EF;
using Lens.Core.Blob.Services;

namespace Lens.Core.Blob
{
    public static class BlobServiceBuilderExtensions
    {
        public static IApplicationSetupBuilder AddBlobService(this IApplicationSetupBuilder builder, 
            BlobImplementationType blobImplementationType,
            string connectionStringName = "DefaultConnection",
            string connectionStringPassword = "dbPassword")
        {
            var blobSettings = builder.Configuration.GetSection(nameof(BlobSettings)).Get<BlobSettings>();
            if (blobSettings == null)
            {
                throw new NotFoundException($"Missing '{nameof(BlobSettings)}' configuration section.");
            }
            if (string.IsNullOrEmpty(blobSettings.ContainerPath))
            {
                throw new NotFoundException($"Missing value for '{nameof(BlobSettings.ContainerPath)}'");
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

            builder
                .AddProgramInitializer<BlobInitializerService>()
                .AddAssemblies(typeof(AutoMapperProfile).Assembly)
                .AddDatabase<BlobDbContext>(connectionStringName, connectionStringPassword)
                .Services
                .AddScoped<IBlobManagementService, BlobManagementService>();

            return builder;
        }
    }
}
