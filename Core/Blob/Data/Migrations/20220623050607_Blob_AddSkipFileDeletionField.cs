using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lens.Core.Blob.Data.Migrations
{
    public partial class Blob_AddSkipFileDeletionField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SkipFileDeletion",
                schema: "blob",
                table: "BlobInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkipFileDeletion",
                schema: "blob",
                table: "BlobInfo");
        }
    }
}
