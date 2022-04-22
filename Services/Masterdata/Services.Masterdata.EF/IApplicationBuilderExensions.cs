using Lens.Core.Data.EF;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Lens.Services.Masterdata.EF.Services;
using Lens.Services.Masterdata.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Services.Masterdata.EF;

public static class IApplicationBuilderExensions
{
    public static IApplicationSetupBuilder AddMasterdata(this IApplicationSetupBuilder builder)
    {
        builder
            .AddAssemblies(typeof(IApplicationBuilderExensions).Assembly)
            .AddDatabase<MasterdataDbContext>()
            .AddProgramInitializer<DatabaseInitializerService<MasterdataDbContext>>()
            .Services
                .AddScoped<IMasterdataService, MasterdataService>();

        return builder;
    }
}
