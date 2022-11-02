using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lens.Services.Masterdata.EF.Migrations
{
    public partial class AddRecordState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecordState",
                table: "MasterdataTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecordState",
                table: "Masterdatas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordState",
                table: "MasterdataTypes");

            migrationBuilder.DropColumn(
                name: "RecordState",
                table: "Masterdatas");
        }
    }
}
