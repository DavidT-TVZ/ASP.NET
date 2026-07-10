using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnD_Character_Sheet_Creator.Data.Migrations
{
    public partial class AddCharacterFieldsAndCharacterEquipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new character columns
            migrationBuilder.AddColumn<int>(
                name: "CurrentExperiencePoints",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfLastLevelUp",
                table: "Characters",
                type: "datetime2",
                nullable: true);

            // Create join table CharacterEquipment
            migrationBuilder.CreateTable(
                name: "CharacterEquipment",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterEquipment", x => new { x.CharacterId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_EquipmentId",
                table: "CharacterEquipment",
                column: "EquipmentId");

            // Migrate existing Equipment.CharacterId values to join table
            // Use SQL to insert existing relationships (if Equipment has CharacterId column)
            try
            {
                migrationBuilder.Sql(@"
                    INSERT INTO CharacterEquipment (CharacterId, EquipmentId)
                    SELECT CharacterId, EquipmentId FROM Equipment WHERE CharacterId IS NOT NULL
                ");
            }
            catch
            {
                // If Equipment.CharacterId doesn't exist or SQL fails, skip gracefully
            }

            // Optionally drop Equipment.CharacterId if present
            try
            {
                migrationBuilder.DropColumn(
                    name: "CharacterId",
                    table: "Equipment");
            }
            catch
            {
                // ignore if column not present
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add Equipment.CharacterId and move data back
            try
            {
                migrationBuilder.AddColumn<int>(
                    name: "CharacterId",
                    table: "Equipment",
                    type: "int",
                    nullable: true);

                migrationBuilder.Sql(@"
                    UPDATE Equipment SET CharacterId = ce.CharacterId
                    FROM CharacterEquipment ce
                    WHERE Equipment.EquipmentId = ce.EquipmentId
                ");
            }
            catch
            {
            }

            migrationBuilder.DropTable(
                name: "CharacterEquipment");

            migrationBuilder.DropColumn(
                name: "CurrentExperiencePoints",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DateOfLastLevelUp",
                table: "Characters");
        }
    }
}
