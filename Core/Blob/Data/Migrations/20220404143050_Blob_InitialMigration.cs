using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lens.Core.Blob.Data.Migrations
{
    public partial class Blob_InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "blob");

            migrationBuilder.CreateTable(
                name: "BlobInfo",
                schema: "blob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    ContentType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FilenameWithExtension = table.Column<string>(type: "nvarchar(532)", maxLength: 532, nullable: true),
                    FilenameWithoutExtension = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FullPathAndName = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    RelativePathAndName = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Size = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlobInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlobInfo",
                schema: "blob");
        }
    }
}
