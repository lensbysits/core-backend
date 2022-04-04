using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;
using System.Linq;

namespace Lens.Core.Data.EF
{
    public static class MigrationBuilderExtensions
    {
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
            string path = Path.Combine(data,dirName);

            var sqlFiles = Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories);
            if (!(sqlFiles?.Any() ?? false))
            {
                // TODO: I usually replace this generic exception with `AppMigrationException`.
                throw new Exception($"No sql files found in '{path}'. Please add files or update directory name");
            }

            foreach(var file in sqlFiles)
            {
                builder.Sql(File.ReadAllText(file));
            }

            return builder;
        }
    }
}
