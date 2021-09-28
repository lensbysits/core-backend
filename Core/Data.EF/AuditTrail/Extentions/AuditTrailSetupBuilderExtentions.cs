using CoreLib;
using CoreLib.Builders;
using CoreLib.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApp.Data.AuditTrail
{
    public static class AuditTrailSetupBuilderExtentions
    {
        public static IApplicationSetupBuilder AddAuditTrailing(this IApplicationSetupBuilder builder, 
            string connectionStringName = "DefaultConnection", 
            string connectionStringPassword = "dbPassword")
        {
            builder
                .AddProgramInitializer<AuditTrailInitializerService>()
                .AddDatabase<AuditTrailDbContext>(connectionStringName, connectionStringPassword)
                .Services
                .AddAutoMapper(typeof(AutoMapperProfile).Assembly)
                .AddScoped<IAuditTrailService, AuditTrailService>();

            return builder;
        }
    }
}
