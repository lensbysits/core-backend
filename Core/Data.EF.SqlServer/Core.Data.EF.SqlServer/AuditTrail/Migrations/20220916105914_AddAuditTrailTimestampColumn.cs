using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lens.Core.Data.EF.AuditTrail.Migrations
{
    public partial class AddAuditTrailTimestampColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "audittrail",
                table: "EntityChange",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "audittrail",
                table: "EntityChange");
        }
    }
}
