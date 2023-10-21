using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSearch.Infrastructure.Persistence.Migrations
{
    public partial class Replacingcomponentstoclassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Components",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Classification",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Classification",
                table: "Products");

            migrationBuilder.AddColumn<List<string>>(
                name: "Components",
                table: "Products",
                type: "text[]",
                nullable: false);
        }
    }
}
