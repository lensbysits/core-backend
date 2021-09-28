using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;
using System.Linq;

namespace CoreApp.Data
{
    public static class MigrationBuilderExtentions
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
    }
}
