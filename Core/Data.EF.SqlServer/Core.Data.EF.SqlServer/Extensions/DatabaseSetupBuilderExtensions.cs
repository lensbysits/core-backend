using Lens.Core.Data.EF.Extensions;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib.Builders;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Lens.Core.Data.EF;

public static class DatabaseSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddSqlServerDatabase<TContext>(this IApplicationSetupBuilder builder,
        string connectionStringName = "DefaultConnection",
        string connectionStringPassword = "dbPassword") where TContext : DbContext
    {
        // connect to a SQL Server database
        var connectionStringBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString(connectionStringName));
        if (string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
        {
            var password = builder.Configuration.GetValue<string>(connectionStringPassword);
            if (string.IsNullOrEmpty(password))
            {
                Log.Warning($"{connectionStringPassword} is empty, connectionstring '{connectionStringName}' has no password.");
            }
            else
            {
                connectionStringBuilder.Password = password;
            }
        }

        var connectionString = connectionStringBuilder.ConnectionString;
        builder.Services.AddDbContext<TContext>(dbOptions =>
        {
            dbOptions.UseSqlServer(connectionString);
        });

        builder
            .AddModelBuilderService()
            .Services
                .AddTransient<IModelBuilderService, IdModelBuilderService>();

        return builder;
    }
}
