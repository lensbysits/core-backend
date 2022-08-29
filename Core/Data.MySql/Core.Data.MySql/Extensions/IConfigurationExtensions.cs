using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Serilog;

namespace Lens.Core.Data.EF.MySql.Extensions;

public static class IConfigurationExtensions
{
    public static (string connectionString, string database) ParseMySqlConnectionString(
        this IConfiguration configuration,
        string connectionStringName = "DefaultConnection",
        string connectionStringPassword = "dbPassword",
        bool removeDatabase = false, 
        bool? allowUserVariables = null)
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder(configuration.GetConnectionString(connectionStringName));
        if (string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
        {
            var password = configuration.GetValue<string>(connectionStringPassword);
            if (string.IsNullOrEmpty(password))
            {
                Log.Warning($"{connectionStringPassword} is empty, connectionstring '{connectionStringName}' has no password.");
            }
            else
            {
                connectionStringBuilder.Password = password;
            }
        }

        if (allowUserVariables.HasValue)
            connectionStringBuilder.AllowUserVariables = allowUserVariables.Value;

        if (removeDatabase)
            connectionStringBuilder.Database = null;

        var connectionString = connectionStringBuilder.ConnectionString;
        return (connectionString, connectionStringBuilder.Database);
    }
}
