using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DnD_Character_Sheet_Creator.Migrations
{
    /// <inheritdoc />
    public partial class AlignModelForSeedAndSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Characters_CharacterId",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_CharacterId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "Equipment");

            migrationBuilder.AddColumn<int>(
                name: "ArmourClass",
                table: "Equipment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DamageAmount",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DamageType",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Equipment",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "StealthDisadvantage",
                table: "Equipment",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StrenghtRequierment",
                table: "Equipment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeaponProperties",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AcrobaticsProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AnimalHandlingProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ArcanaProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AthleticsProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Charisma",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CharismaSaveProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Constitution",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ConstitutionSaveProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<bool>(
                name: "DeceptionProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Dexterity",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DexteritySaveProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HistoryProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InsightProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Intelligence",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IntelligenceSaveProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntimidationProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InvestigationProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MedicineProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NatureProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerceptionProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerformanceProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PersuasionProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReligionProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SleightOfHandProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StealthProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Strength",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "StrengthSaveProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SurvivalProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Wisdom",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WisdomSaveProficient",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            // Insert character levels only if they don't already exist
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM CharacterLevels WHERE LevelId = 1)
BEGIN
    INSERT INTO CharacterLevels (LevelId, CurrentExperiencePoints, DateOfLastLevelUp, ExperiencePointsToNextLevel, Level, ProficiencyBonus)
    VALUES (1, 0, '2026-07-10T02:09:57.5287680Z', 300, 1, 2)
END
IF NOT EXISTS (SELECT 1 FROM CharacterLevels WHERE LevelId = 2)
BEGIN
    INSERT INTO CharacterLevels (LevelId, CurrentExperiencePoints, DateOfLastLevelUp, ExperiencePointsToNextLevel, Level, ProficiencyBonus)
    VALUES (2, 300, '2026-07-10T02:09:57.5287875Z', 900, 2, 2)
END
IF NOT EXISTS (SELECT 1 FROM CharacterLevels WHERE LevelId = 3)
BEGIN
    INSERT INTO CharacterLevels (LevelId, CurrentExperiencePoints, DateOfLastLevelUp, ExperiencePointsToNextLevel, Level, ProficiencyBonus)
    VALUES (3, 900, '2026-07-10T02:09:57.5287878Z', 2700, 3, 2)
END
IF NOT EXISTS (SELECT 1 FROM CharacterLevels WHERE LevelId = 4)
BEGIN
    INSERT INTO CharacterLevels (LevelId, CurrentExperiencePoints, DateOfLastLevelUp, ExperiencePointsToNextLevel, Level, ProficiencyBonus)
    VALUES (4, 2700, '2026-07-10T02:09:57.5287879Z', 6500, 4, 2)
END
IF NOT EXISTS (SELECT 1 FROM CharacterLevels WHERE LevelId = 5)
BEGIN
    INSERT INTO CharacterLevels (LevelId, CurrentExperiencePoints, DateOfLastLevelUp, ExperiencePointsToNextLevel, Level, ProficiencyBonus)
    VALUES (5, 6500, '2026-07-10T02:09:57.5287880Z', 14000, 5, 3)
END
" );

            // Insert equipment only if not present
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Equipment WHERE EquipmentId = 1001)
BEGIN
    SET IDENTITY_INSERT Equipment ON;
    INSERT INTO Equipment (EquipmentId, Cost, DamageAmount, DamageType, DeletedAt, Discriminator, Name, Type, WeaponProperties, Weight)
    VALUES (1001, 15, '1d8', 'Slashing', NULL, 'Weapon', 'Longsword', 'Weapon', '[]', 3);
    SET IDENTITY_INSERT Equipment OFF;
END
" );

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Equipment WHERE EquipmentId = 1002)
BEGIN
    SET IDENTITY_INSERT Equipment ON;
    INSERT INTO Equipment (EquipmentId, ArmourClass, Cost, DeletedAt, Discriminator, Name, StealthDisadvantage, StrenghtRequierment, Type, Weight)
    VALUES (1002, 0, 75, NULL, 'Armour', 'Chain Mail', 0, 0, 'Armour', 55);
    SET IDENTITY_INSERT Equipment OFF;
END
" );

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Equipment WHERE EquipmentId = 1003)
BEGIN
    SET IDENTITY_INSERT Equipment ON;
    INSERT INTO Equipment (EquipmentId, Cost, DeletedAt, Discriminator, Name, Type, Weight)
    VALUES (1003, 2, NULL, 'AdventuringGear', 'Backpack', 'Gear', 5);
    SET IDENTITY_INSERT Equipment OFF;
END
" );

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Equipment WHERE EquipmentId = 1004)
BEGIN
    SET IDENTITY_INSERT Equipment ON;
    INSERT INTO Equipment (EquipmentId, Cost, DamageAmount, DamageType, DeletedAt, Discriminator, Name, Type, WeaponProperties, Weight)
    VALUES (1004, 25, '1d8', 'Piercing', NULL, 'Weapon', 'Rapier', 'Weapon', '[]', 2);
    SET IDENTITY_INSERT Equipment OFF;
END
" );

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Equipment WHERE EquipmentId = 1015)
BEGIN
    SET IDENTITY_INSERT Equipment ON;
    INSERT INTO Equipment (EquipmentId, Cost, DeletedAt, Discriminator, Name, Type, Weight)
    VALUES (1015, 25, NULL, 'AdventuringGear', 'Thieves'' Tools', 'Gear', 1);
    SET IDENTITY_INSERT Equipment OFF;
END
" );

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Equipment WHERE EquipmentId = 1025)
BEGIN
    SET IDENTITY_INSERT Equipment ON;
    INSERT INTO Equipment (EquipmentId, Cost, DamageAmount, DamageType, DeletedAt, Discriminator, Name, Type, WeaponProperties, Weight)
    VALUES (1025, 2, '1d4', 'Piercing', NULL, 'Weapon', 'Dagger', 'Weapon', '[]', 1);
    SET IDENTITY_INSERT Equipment OFF;
END
" );

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_EquipmentId",
                table: "CharacterEquipment",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterEquipment");

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1025);

            migrationBuilder.DropColumn(
                name: "ArmourClass",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "DamageAmount",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "DamageType",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "StealthDisadvantage",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "StrenghtRequierment",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "WeaponProperties",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "AcrobaticsProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AnimalHandlingProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ArcanaProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AthleticsProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Charisma",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CharismaSaveProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Constitution",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ConstitutionSaveProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentExperiencePoints",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DateOfLastLevelUp",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DeceptionProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Dexterity",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DexteritySaveProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "HistoryProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "InsightProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Intelligence",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IntelligenceSaveProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IntimidationProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "InvestigationProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MedicineProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "NatureProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PerceptionProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PerformanceProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PersuasionProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ReligionProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SleightOfHandProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "StealthProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "StrengthSaveProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SurvivalProficient",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Wisdom",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "WisdomSaveProficient",
                table: "Characters");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "Equipment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CharacterId",
                table: "Equipment",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Characters_CharacterId",
                table: "Equipment",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
