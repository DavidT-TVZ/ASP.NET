using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnD_Character_Sheet_Creator.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsAdminWithRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Players");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Players");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
