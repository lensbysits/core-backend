using Lens.Core.Data.EF.AuditTrail.Services;
using Lens.Core.Data.Services;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Data.EF.AuditTrail
{
    public static class AuditTrailSetupBuilderExtensions
    {
        public static IApplicationSetupBuilder AddAuditTrailing(this IApplicationSetupBuilder builder, 
            string connectionStringName = "DefaultConnection", 
            string connectionStringPassword = "dbPassword")
        {
            builder
                .AddProgramInitializer<AuditTrailInitializerService>()
                .AddAssemblies(typeof(AutoMapperProfile).Assembly)
                .AddDatabase<AuditTrailDbContext>(connectionStringName, connectionStringPassword)
                .Services
                    .AddScoped<IAuditTrailService, AuditTrailService>();

            return builder;
        }
    }
}
