using CoreLib;
using CoreLib.Builders;
using CoreLib.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApp.Data.AuditTrail
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
