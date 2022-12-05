using Microsoft.Extensions.DependencyInjection;
using Lens.Core.Data.EF;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Lens.Services.Masterdata.Services;
using Lens.Services.Masterdata.Repositories;
using Lens.Services.Masterdata.EF.Repositories;

namespace Lens.Services.Masterdata.EF;

public static class IApplicationBuilderExensions
{
    public static IApplicationSetupBuilder AddMasterdata(this IApplicationSetupBuilder builder)
    {
        builder
            .AddAssemblies(typeof(IApplicationBuilderExensions).Assembly)
            .AddSqlServerDatabase<MasterdataDbContext>()
            .AddProgramInitializer<DatabaseInitializerService<MasterdataDbContext>>()
            .Services
                .AddScoped<IMasterdataRepository, MasterdataRepository>()
                .AddScoped<IMasterdataService, MasterdataService>();

        return builder;
    }
}
