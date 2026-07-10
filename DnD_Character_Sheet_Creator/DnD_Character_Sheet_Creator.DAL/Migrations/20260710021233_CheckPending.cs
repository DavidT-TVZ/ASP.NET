using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnD_Character_Sheet_Creator.Migrations
{
    /// <inheritdoc />
    public partial class CheckPending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 1,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 12, 32, 764, DateTimeKind.Utc).AddTicks(3871));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 2,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 12, 32, 764, DateTimeKind.Utc).AddTicks(4068));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 3,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 12, 32, 764, DateTimeKind.Utc).AddTicks(4069));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 4,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 12, 32, 764, DateTimeKind.Utc).AddTicks(4071));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 5,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 12, 32, 764, DateTimeKind.Utc).AddTicks(4072));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 1,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 9, 57, 528, DateTimeKind.Utc).AddTicks(7680));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 2,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 9, 57, 528, DateTimeKind.Utc).AddTicks(7875));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 3,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 9, 57, 528, DateTimeKind.Utc).AddTicks(7878));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 4,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 9, 57, 528, DateTimeKind.Utc).AddTicks(7879));

            migrationBuilder.UpdateData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 5,
                column: "DateOfLastLevelUp",
                value: new DateTime(2026, 7, 10, 2, 9, 57, 528, DateTimeKind.Utc).AddTicks(7880));
        }
    }
}
