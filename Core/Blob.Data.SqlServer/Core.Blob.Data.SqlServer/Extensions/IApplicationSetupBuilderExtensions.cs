using Lens.Core.Blob.Data;
using Lens.Core.Blob.Services;
using Lens.Core.Data.EF;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Blob;

public static class IApplicationSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddBlobManagementService(this IApplicationSetupBuilder builder,
        BlobImplementationType blobImplementationType,
        string connectionStringName = "DefaultConnection",
        string connectionStringPassword = "dbPassword")
    {
        builder.AddBlobService(blobImplementationType);

        builder
            .AddProgramInitializer<BlobInitializerService>()
            .AddAssemblies(typeof(AutoMapperProfile).Assembly)
            .AddSqlServerDatabase<BlobDbContext>(connectionStringName, connectionStringPassword)
            .Services
                .AddScoped<IBlobManagementService, BlobManagementService>();

        return builder;
    }
}
