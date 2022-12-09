using Lens.Core.Data.EF.AuditTrail.RowLevelSecurity.Providers;
using Lens.Core.Data.EF.AuditTrail.Services;
using Lens.Core.Data.EF.Configuration;
using Lens.Core.Data.EF.RowLevelSecurity.Interceptors;
using Lens.Core.Data.Services;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Data.EF.AuditTrail;

public static class AuditTrailSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddAuditTrailing(this IApplicationSetupBuilder builder,
        string connectionStringName = "DefaultConnection",
        string connectionStringPassword = "dbPassword")
    {
        builder
            .AddProgramInitializer<AuditTrailInitializerService>()
            .AddAssemblies(typeof(AutoMapperProfile).Assembly)
            .AddSqlServerDatabase<AuditTrailDbContext>(connectionStringName, connectionStringPassword, typeof(AuditTrailSetupBuilderExtensions).Assembly)
            .Services
                .AddScoped<IAuditTrailService, AuditTrailService>()
                .AddScoped<IDbCommandInterceptor, RowLevelSecurityInterceptor>();

        return builder;
    }
    /// <summary>
    /// Add audit trailing that's configured to work with Row Level Security (rls) by providing the rls identity on data read and write.
    /// </summary>
    /// <typeparam name="T">Your custom logic to get the rls identity for the current user.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="connectionStringName">The connection string name.</param>
    /// <param name="connectionStringPassword">the connection string password.</param>
    public static IApplicationSetupBuilder AddAuditTrailingWithRlsSupport<T>(this IApplicationSetupBuilder builder,
       string connectionStringName = "DefaultConnection",
       string connectionStringPassword = "dbPassword") where T : class, IAuditRlsIdentityProvider
    {
        builder.Services.Configure<AuditSettings>(options => builder.Configuration.Bind(nameof(AuditSettings), options));
        builder.Services.AddTransient<IAuditRlsIdentityProvider, T>();
        return builder.AddAuditTrailing(connectionStringName, connectionStringPassword);
    }
}
