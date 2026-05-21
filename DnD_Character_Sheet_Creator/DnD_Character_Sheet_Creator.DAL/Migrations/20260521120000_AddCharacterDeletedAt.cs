using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnD_Character_Sheet_Creator.Migrations
{
    public partial class AddCharacterDeletedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Characters",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Characters");
        }
    }
}