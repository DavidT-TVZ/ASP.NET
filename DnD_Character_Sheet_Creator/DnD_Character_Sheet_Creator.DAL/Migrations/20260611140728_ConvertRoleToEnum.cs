using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnD_Character_Sheet_Creator.Migrations
{
    /// <inheritdoc />
    public partial class ConvertRoleToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, update the string values to temporary numeric values
            migrationBuilder.Sql("UPDATE Players SET Role = '0' WHERE Role = 'Admin'");
            migrationBuilder.Sql("UPDATE Players SET Role = '1' WHERE Role = 'Manager'");
            migrationBuilder.Sql("UPDATE Players SET Role = '2' WHERE Role = 'User'");
            
            // Then, alter the column type from nvarchar(max) to int
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Players",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
