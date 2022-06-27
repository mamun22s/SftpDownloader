using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActiveXTService.Data.Migrations
{
    public partial class modelchanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogTime",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ImportedFiles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogDate",
                table: "Logs",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ImportedFiles",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "FileSaveDate",
                table: "ImportedFiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSaveDate",
                table: "ImportedFiles");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LogDate",
                table: "Logs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "LogTime",
                table: "Logs",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CreatedDate",
                table: "ImportedFiles",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CreatedTime",
                table: "ImportedFiles",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
