using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lens.Core.Blob.Data.Migrations;

public partial class Blob_AddTagOnBlobInfo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "blob",
            table: "BlobInfo",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedOn",
            schema: "blob",
            table: "BlobInfo",
            type: "datetime2",
            nullable: false,
            defaultValueSql: "GETUTCDATE()");

        migrationBuilder.AddColumn<int>(
            name: "RecordState",
            schema: "blob",
            table: "BlobInfo",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Tag",
            schema: "blob",
            table: "BlobInfo",
            type: "nvarchar(2048)",
            maxLength: 2048,
            nullable: true);

        migrationBuilder.AddColumn<byte[]>(
            name: "Timestamp",
            schema: "blob",
            table: "BlobInfo",
            type: "rowversion",
            rowVersion: true,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "blob",
            table: "BlobInfo",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedOn",
            schema: "blob",
            table: "BlobInfo",
            type: "datetime2",
            nullable: false,
            defaultValueSql: "GETUTCDATE()");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "blob",
            table: "BlobInfo");

        migrationBuilder.DropColumn(
            name: "CreatedOn",
            schema: "blob",
            table: "BlobInfo");

        migrationBuilder.DropColumn(
            name: "RecordState",
            schema: "blob",
            table: "BlobInfo");

        migrationBuilder.DropColumn(
            name: "Tag",
            schema: "blob",
            table: "BlobInfo");

        migrationBuilder.DropColumn(
            name: "Timestamp",
            schema: "blob",
            table: "BlobInfo");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "blob",
            table: "BlobInfo");

        migrationBuilder.DropColumn(
            name: "UpdatedOn",
            schema: "blob",
            table: "BlobInfo");
    }
}
