using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActiveXTService.Data.Migrations
{
    public partial class newDataadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "CreatedTime",
                table: "ImportedFiles",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ImportedFiles");
        }
    }
}
