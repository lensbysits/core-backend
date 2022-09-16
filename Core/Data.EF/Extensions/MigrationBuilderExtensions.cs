using Microsoft.EntityFrameworkCore.Migrations;

namespace Lens.Core.Data.EF;

public static class MigrationBuilderExtensions
{
    /// <summary>
    /// All sql below this header in a sql file will be run in the Migration.Down() method.
    /// A SQL file must always contain the following structure:
    /// --efcore.migration.down
    ///     All the sql statements to revert the changes the file creates
    ///     
    /// --efcore.migration.up
    ///     All the sql statements to execute the database changes
    /// </summary>
    private const string SqlFileHeaderMigrationDown = "--efcore.migration.down";

    /// <summary>
    /// All sql below this header in a sql file will be run in the Migration.Up() method
    /// </summary>
    private const string SqlFileHeaderMigrationUp = "--efcore.migration.up";


    public static MigrationBuilder DropStoredProcedureIfExists(this MigrationBuilder builder, string storedProcedureName, string schemaName = "dbo")
    {
        builder.Sql(
        $@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('{schemaName}.{storedProcedureName}'))
                DROP PROCEDURE [{schemaName}].[{storedProcedureName}]
            GO");
        return builder;
    }

    public static MigrationBuilder DropViewIfExists(this MigrationBuilder builder, string viewName, string schemaName = "dbo")
    {
        builder.Sql(
        $@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'V' AND OBJECT_ID = OBJECT_ID('{schemaName}.{viewName}'))
                DROP VIEW [{schemaName}].[{viewName}]
            GO");
        return builder;
    }

    public static MigrationBuilder RunFile(this MigrationBuilder builder, string filename)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string ?? AppContext.BaseDirectory;

        var sqlFiles = Directory.GetFiles(data, filename, SearchOption.AllDirectories);
        if (sqlFiles?.Any() ?? false)
        {
            builder.Sql(File.ReadAllText(sqlFiles.First()));
        }
        else
        {
            // TODO: I usually replace this generic exception with `AppMigrationException`.
            throw new Exception($"Migration .sql file not found: ${filename}");
        }
        return builder;
    }

    public static MigrationBuilder RunFiles(this MigrationBuilder builder, string dirName)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string ?? AppContext.BaseDirectory;
        string path = Path.Combine(data, dirName);

        var sqlFiles = Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories);
        if (!(sqlFiles?.Any() ?? false))
        {
            // TODO: I usually replace this generic exception with `AppMigrationException`.
            throw new Exception($"No sql files found in '{path}'. Please add files or update directory name");
        }

        foreach (var file in sqlFiles)
        {
            using var reader = new StreamReader(file);
            var firstLine = reader.ReadLine();
            if (!(firstLine?.Equals(SqlFileHeaderMigrationDown) ?? false))
            {
                throw new Exception($"Headers missing for bulk file run. Please add the 2 required headers: --efcore.migration.down and --efcore.migration.up. For file: {file}");
            }

            var line = string.Empty;
            do
            {
                line = reader.ReadLine();
            } while (!(line?.Equals(SqlFileHeaderMigrationUp) ?? true));

            var sql = reader.ReadToEnd();
            builder.Sql(sql);
            reader.Close();
        }

        return builder;
    }

    public static MigrationBuilder RevertFiles(this MigrationBuilder builder, string dirName)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string ?? AppContext.BaseDirectory;
        string path = Path.Combine(data, dirName);

        var sqlFiles = Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories);
        if (!(sqlFiles?.Any() ?? false))
        {
            // TODO: I usually replace this generic exception with `AppMigrationException`.
            throw new Exception($"No sql files found in '{path}'. Please add files or update directory name");
        }

        foreach (var file in sqlFiles)
        {
            using var reader = new StreamReader(file);
            var firstLine = reader.ReadLine();
            if (!(firstLine?.Equals("--efcore.migration.down") ?? false))
            {
                throw new Exception("Headers missing for bulk file run. Please add the 2 required headers: --efcore.migration.down and --efcore.migration.up");
            }

            var line = string.Empty;

            while (!(line?.Equals(SqlFileHeaderMigrationUp) ?? true))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var uncommentedSql = line[2..];
                    builder.Sql(uncommentedSql);
                }

                line = reader.ReadLine();
            }
        }

        return builder;
    }
}
