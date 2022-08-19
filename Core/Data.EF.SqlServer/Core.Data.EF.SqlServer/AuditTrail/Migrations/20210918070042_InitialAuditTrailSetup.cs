using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lens.Core.Data.EF.AuditTrail.Migrations
{
    public partial class InitialAuditTrailSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audittrail");

            migrationBuilder.CreateTable(
                name: "EntityChange",
                schema: "audittrail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    ChangeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangeToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityChange", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityChange",
                schema: "audittrail");
        }
    }
}
