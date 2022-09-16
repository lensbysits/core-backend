using Lens.Core.Data.EF.Extensions;
using Lens.Core.Data.EF.MySql.Extensions;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Data.EF;

public static class DatabaseSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddMySqlDatabase<TContext>(this IApplicationSetupBuilder builder,
        string connectionStringName = "DefaultConnection",
        string connectionStringPassword = "dbPassword",
        Action<string, string>? connectionInfo = null) where TContext : DbContext
    {
        var (connectionString, database) = builder.Configuration.ParseMySqlConnectionString(connectionStringName, connectionStringPassword);
        var version = ServerVersion.AutoDetect(connectionString);
        builder.Services.AddDbContext<TContext>(dbOptions =>
        {
            dbOptions.UseMySql(connectionString, version);
        });

        builder
            .AddModelBuilderService()
            .Services
                .AddTransient<IModelBuilderService, IdModelBuilderService>();

        connectionInfo?.Invoke(connectionString, database);
        return builder;
    }
}
