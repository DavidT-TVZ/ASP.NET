using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DnD_Character_Sheet_Creator.Migrations
{
    /// <inheritdoc />
    public partial class SeedPhbEquipmentCategoriesAndTools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'[CharacterLevels]') AND [name] = 'LevelId')
    SET IDENTITY_INSERT [CharacterLevels] ON;

MERGE [CharacterLevels] AS target
USING (VALUES
    (1, 0, 300, 1, 2, CAST('2026-07-10T00:00:00' AS datetime2)),
    (2, 300, 900, 2, 2, CAST('2026-07-10T00:00:00' AS datetime2)),
    (3, 900, 2700, 3, 2, CAST('2026-07-10T00:00:00' AS datetime2)),
    (4, 2700, 6500, 4, 2, CAST('2026-07-10T00:00:00' AS datetime2)),
    (5, 6500, 14000, 5, 3, CAST('2026-07-10T00:00:00' AS datetime2)),
    (6, 14000, 23000, 6, 3, CAST('2026-07-10T00:00:00' AS datetime2)),
    (7, 23000, 34000, 7, 3, CAST('2026-07-10T00:00:00' AS datetime2)),
    (8, 34000, 48000, 8, 3, CAST('2026-07-10T00:00:00' AS datetime2)),
    (9, 48000, 64000, 9, 4, CAST('2026-07-10T00:00:00' AS datetime2)),
    (10, 64000, 85000, 10, 4, CAST('2026-07-10T00:00:00' AS datetime2)),
    (11, 85000, 100000, 11, 4, CAST('2026-07-10T00:00:00' AS datetime2)),
    (12, 100000, 120000, 12, 4, CAST('2026-07-10T00:00:00' AS datetime2)),
    (13, 120000, 140000, 13, 5, CAST('2026-07-10T00:00:00' AS datetime2)),
    (14, 140000, 165000, 14, 5, CAST('2026-07-10T00:00:00' AS datetime2)),
    (15, 165000, 195000, 15, 5, CAST('2026-07-10T00:00:00' AS datetime2)),
    (16, 195000, 225000, 16, 5, CAST('2026-07-10T00:00:00' AS datetime2)),
    (17, 225000, 265000, 17, 6, CAST('2026-07-10T00:00:00' AS datetime2)),
    (18, 265000, 305000, 18, 6, CAST('2026-07-10T00:00:00' AS datetime2)),
    (19, 305000, 355000, 19, 6, CAST('2026-07-10T00:00:00' AS datetime2)),
    (20, 355000, 0, 20, 6, CAST('2026-07-10T00:00:00' AS datetime2))
) AS source ([LevelId], [CurrentExperiencePoints], [ExperiencePointsToNextLevel], [Level], [ProficiencyBonus], [DateOfLastLevelUp])
ON target.[LevelId] = source.[LevelId]
WHEN MATCHED THEN
    UPDATE SET
        target.[CurrentExperiencePoints] = source.[CurrentExperiencePoints],
        target.[ExperiencePointsToNextLevel] = source.[ExperiencePointsToNextLevel],
        target.[Level] = source.[Level],
        target.[ProficiencyBonus] = source.[ProficiencyBonus],
        target.[DateOfLastLevelUp] = COALESCE(target.[DateOfLastLevelUp], source.[DateOfLastLevelUp])
WHEN NOT MATCHED THEN
    INSERT ([LevelId], [CurrentExperiencePoints], [DateOfLastLevelUp], [ExperiencePointsToNextLevel], [Level], [ProficiencyBonus])
    VALUES (source.[LevelId], source.[CurrentExperiencePoints], source.[DateOfLastLevelUp], source.[ExperiencePointsToNextLevel], source.[Level], source.[ProficiencyBonus]);

IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'[CharacterLevels]') AND [name] = 'LevelId')
    SET IDENTITY_INSERT [CharacterLevels] OFF;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'[Equipment]') AND [name] = 'EquipmentId')
    SET IDENTITY_INSERT [Equipment] ON;

MERGE [Equipment] AS target
USING (VALUES
    (1001, N'Weapon', N'Club', N'Simple Melee Weapon', 0, 2, N'1d4', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1002, N'Weapon', N'Dagger', N'Simple Melee Weapon', 2, 1, N'1d4', N'Piercing', N'[]', NULL, NULL, NULL),
    (1003, N'Weapon', N'Greatclub', N'Simple Melee Weapon', 0, 10, N'1d8', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1004, N'Weapon', N'Handaxe', N'Simple Melee Weapon', 5, 2, N'1d6', N'Slashing', N'[]', NULL, NULL, NULL),
    (1005, N'Weapon', N'Javelin', N'Simple Melee Weapon', 5, 2, N'1d6', N'Piercing', N'[]', NULL, NULL, NULL),
    (1006, N'Weapon', N'Light Hammer', N'Simple Melee Weapon', 2, 2, N'1d4', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1007, N'Weapon', N'Mace', N'Simple Melee Weapon', 5, 4, N'1d6', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1008, N'Weapon', N'Quarterstaff', N'Simple Melee Weapon', 0, 4, N'1d6', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1009, N'Weapon', N'Sickle', N'Simple Melee Weapon', 1, 2, N'1d4', N'Slashing', N'[]', NULL, NULL, NULL),
    (1010, N'Weapon', N'Spear', N'Simple Melee Weapon', 1, 3, N'1d6', N'Piercing', N'[]', NULL, NULL, NULL),
    (1011, N'Weapon', N'Light Crossbow', N'Simple Ranged Weapon', 25, 5, N'1d8', N'Piercing', N'[]', NULL, NULL, NULL),
    (1012, N'Weapon', N'Shortbow', N'Simple Ranged Weapon', 25, 2, N'1d6', N'Piercing', N'[]', NULL, NULL, NULL),
    (1013, N'Weapon', N'Longsword', N'Martial Melee Weapon', 15, 3, N'1d8', N'Slashing', N'[]', NULL, NULL, NULL),
    (1014, N'Weapon', N'Shortsword', N'Martial Melee Weapon', 10, 2, N'1d6', N'Piercing', N'[]', NULL, NULL, NULL),
    (1015, N'Weapon', N'Scimitar', N'Martial Melee Weapon', 25, 3, N'1d6', N'Slashing', N'[]', NULL, NULL, NULL),
    (1016, N'Weapon', N'Warhammer', N'Martial Melee Weapon', 15, 5, N'1d8', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1017, N'Weapon', N'Battleaxe', N'Martial Melee Weapon', 10, 4, N'1d8', N'Slashing', N'[]', NULL, NULL, NULL),
    (1018, N'Weapon', N'Greatsword', N'Martial Melee Weapon', 50, 6, N'2d6', N'Slashing', N'[]', NULL, NULL, NULL),
    (1019, N'Weapon', N'Halberd', N'Martial Melee Weapon', 20, 6, N'1d10', N'Slashing', N'[]', NULL, NULL, NULL),
    (1020, N'Weapon', N'Pike', N'Martial Melee Weapon', 5, 18, N'1d10', N'Piercing', N'[]', NULL, NULL, NULL),
    (1021, N'Weapon', N'Maul', N'Martial Melee Weapon', 10, 10, N'2d6', N'Bludgeoning', N'[]', NULL, NULL, NULL),
    (1022, N'Weapon', N'Rapier', N'Martial Melee Weapon', 25, 2, N'1d8', N'Piercing', N'[]', NULL, NULL, NULL),
    (1023, N'Weapon', N'Longbow', N'Martial Ranged Weapon', 50, 2, N'1d8', N'Piercing', N'[]', NULL, NULL, NULL),
    (1024, N'Weapon', N'Heavy Crossbow', N'Martial Ranged Weapon', 50, 18, N'1d10', N'Piercing', N'[]', NULL, NULL, NULL),
    (1025, N'Weapon', N'Throwing Dagger', N'Simple Melee Weapon', 2, 1, N'1d4', N'Piercing', N'[]', NULL, NULL, NULL),
    (1101, N'Armour', N'Padded', N'Light Armour', 5, 8, NULL, NULL, NULL, 11, 0, 0),
    (1102, N'Armour', N'Leather', N'Light Armour', 10, 10, NULL, NULL, NULL, 11, 0, 0),
    (1103, N'Armour', N'Studded Leather', N'Light Armour', 45, 13, NULL, NULL, NULL, 12, 0, 0),
    (1104, N'Armour', N'Hide', N'Medium Armour', 10, 12, NULL, NULL, NULL, 12, 0, 0),
    (1105, N'Armour', N'Chain Shirt', N'Medium Armour', 50, 20, NULL, NULL, NULL, 13, 0, 0),
    (1106, N'Armour', N'Scale Mail', N'Medium Armour', 50, 45, NULL, NULL, NULL, 14, 0, 1),
    (1107, N'Armour', N'Breastplate', N'Medium Armour', 400, 20, NULL, NULL, NULL, 14, 0, 0),
    (1108, N'Armour', N'Half Plate', N'Medium Armour', 750, 40, NULL, NULL, NULL, 15, 0, 1),
    (1109, N'Armour', N'Ring Mail', N'Heavy Armour', 30, 40, NULL, NULL, NULL, 14, 0, 1),
    (1110, N'Armour', N'Chain Mail', N'Heavy Armour', 75, 55, NULL, NULL, NULL, 16, 13, 1),
    (1111, N'Armour', N'Splint', N'Heavy Armour', 200, 60, NULL, NULL, NULL, 17, 0, 1),
    (1112, N'Armour', N'Plate', N'Heavy Armour', 1500, 65, NULL, NULL, NULL, 18, 0, 1),
    (1113, N'Armour', N'Shield', N'Shield', 10, 6, NULL, NULL, NULL, 2, 0, 0),
    (1201, N'AdventuringGear', N'Backpack', N'Gear', 2, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1202, N'AdventuringGear', N'Bedroll', N'Gear', 1, 7, NULL, NULL, NULL, NULL, NULL, NULL),
    (1203, N'AdventuringGear', N'Mess Kit', N'Gear', 2, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1204, N'AdventuringGear', N'Rations (1 day)', N'Gear', 5, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1205, N'AdventuringGear', N'Waterskin', N'Gear', 2, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1206, N'AdventuringGear', N'Hempen Rope (50 ft)', N'Gear', 1, 10, NULL, NULL, NULL, NULL, NULL, NULL),
    (1207, N'AdventuringGear', N'Crowbar', N'Gear', 2, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1208, N'AdventuringGear', N'Hammer', N'Gear', 1, 3, NULL, NULL, NULL, NULL, NULL, NULL),
    (1209, N'AdventuringGear', N'Pitons (10)', N'Gear', 5, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1210, N'AdventuringGear', N'Tinderbox', N'Gear', 5, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1211, N'AdventuringGear', N'Torches (10)', N'Gear', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1212, N'AdventuringGear', N'Hooded Lantern', N'Gear', 5, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1213, N'AdventuringGear', N'Grappling Hook', N'Gear', 2, 4, NULL, NULL, NULL, NULL, NULL, NULL),
    (1214, N'AdventuringGear', N'Mirror (steel)', N'Gear', 5, 0, NULL, NULL, NULL, NULL, NULL, NULL),
    (1215, N'AdventuringGear', N'Chalk (1 piece)', N'Gear', 0, 0, NULL, NULL, NULL, NULL, NULL, NULL),
    (1216, N'AdventuringGear', N'Lantern (bullseye)', N'Gear', 10, 3, NULL, NULL, NULL, NULL, NULL, NULL),
    (1217, N'AdventuringGear', N'Candles (10)', N'Gear', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL),
    (1301, N'Tools', N'Alchemist''s Supplies', N'Artisan''s Tools', 50, 8, NULL, NULL, NULL, NULL, NULL, NULL),
    (1302, N'Tools', N'Brewer''s Supplies', N'Artisan''s Tools', 20, 9, NULL, NULL, NULL, NULL, NULL, NULL),
    (1303, N'Tools', N'Calligrapher''s Supplies', N'Artisan''s Tools', 10, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1304, N'Tools', N'Carpenter''s Tools', N'Artisan''s Tools', 8, 6, NULL, NULL, NULL, NULL, NULL, NULL),
    (1305, N'Tools', N'Cartographer''s Tools', N'Artisan''s Tools', 15, 6, NULL, NULL, NULL, NULL, NULL, NULL),
    (1306, N'Tools', N'Cobbler''s Tools', N'Artisan''s Tools', 5, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1307, N'Tools', N'Cook''s Utensils', N'Artisan''s Tools', 1, 8, NULL, NULL, NULL, NULL, NULL, NULL),
    (1308, N'Tools', N'Glassblower''s Tools', N'Artisan''s Tools', 30, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1309, N'Tools', N'Jeweler''s Tools', N'Artisan''s Tools', 25, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1310, N'Tools', N'Leatherworker''s Tools', N'Artisan''s Tools', 5, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1311, N'Tools', N'Mason''s Tools', N'Artisan''s Tools', 10, 8, NULL, NULL, NULL, NULL, NULL, NULL),
    (1312, N'Tools', N'Painter''s Supplies', N'Artisan''s Tools', 10, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1313, N'Tools', N'Potter''s Tools', N'Artisan''s Tools', 10, 3, NULL, NULL, NULL, NULL, NULL, NULL),
    (1314, N'Tools', N'Smith''s Tools', N'Artisan''s Tools', 20, 8, NULL, NULL, NULL, NULL, NULL, NULL),
    (1315, N'Tools', N'Tinker''s Tools', N'Artisan''s Tools', 50, 10, NULL, NULL, NULL, NULL, NULL, NULL),
    (1316, N'Tools', N'Weaver''s Tools', N'Artisan''s Tools', 1, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1317, N'Tools', N'Woodcarver''s Tools', N'Artisan''s Tools', 1, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1318, N'Tools', N'Dice Set', N'Gaming Set', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL),
    (1319, N'Tools', N'Dragonchess Set', N'Gaming Set', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1320, N'Tools', N'Playing Card Set', N'Gaming Set', 5, 0, NULL, NULL, NULL, NULL, NULL, NULL),
    (1321, N'Tools', N'Three-Dragon Ante Set', N'Gaming Set', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL),
    (1322, N'Tools', N'Bagpipes', N'Musical Instrument', 30, 6, NULL, NULL, NULL, NULL, NULL, NULL),
    (1323, N'Tools', N'Drum', N'Musical Instrument', 6, 3, NULL, NULL, NULL, NULL, NULL, NULL),
    (1324, N'Tools', N'Dulcimer', N'Musical Instrument', 25, 10, NULL, NULL, NULL, NULL, NULL, NULL),
    (1325, N'Tools', N'Flute', N'Musical Instrument', 2, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1326, N'Tools', N'Lute', N'Musical Instrument', 35, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1327, N'Tools', N'Lyre', N'Musical Instrument', 30, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1328, N'Tools', N'Horn', N'Musical Instrument', 3, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1329, N'Tools', N'Pan Flute', N'Musical Instrument', 12, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1330, N'Tools', N'Shawm', N'Musical Instrument', 2, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1331, N'Tools', N'Viol', N'Musical Instrument', 30, 1, NULL, NULL, NULL, NULL, NULL, NULL),
    (1332, N'Tools', N'Disguise Kit', N'Tool Kit', 25, 3, NULL, NULL, NULL, NULL, NULL, NULL),
    (1333, N'Tools', N'Forgery Kit', N'Tool Kit', 15, 5, NULL, NULL, NULL, NULL, NULL, NULL),
    (1334, N'Tools', N'Herbalism Kit', N'Tool Kit', 5, 3, NULL, NULL, NULL, NULL, NULL, NULL),
    (1335, N'Tools', N'Navigator''s Tools', N'Tool Kit', 25, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1336, N'Tools', N'Poisoner''s Kit', N'Tool Kit', 50, 2, NULL, NULL, NULL, NULL, NULL, NULL),
    (1337, N'Tools', N'Thieves'' Tools', N'Tool Kit', 25, 1, NULL, NULL, NULL, NULL, NULL, NULL)
) AS source (
    [EquipmentId], [Discriminator], [Name], [Type], [Cost], [Weight], [DamageAmount], [DamageType], [WeaponProperties], [ArmourClass], [StrenghtRequierment], [StealthDisadvantage]
)
ON target.[EquipmentId] = source.[EquipmentId]
WHEN MATCHED THEN
    UPDATE SET
        target.[Discriminator] = source.[Discriminator],
        target.[Name] = source.[Name],
        target.[Type] = source.[Type],
        target.[Cost] = source.[Cost],
        target.[Weight] = source.[Weight],
        target.[DamageAmount] = source.[DamageAmount],
        target.[DamageType] = source.[DamageType],
        target.[WeaponProperties] = source.[WeaponProperties],
        target.[ArmourClass] = source.[ArmourClass],
        target.[StrenghtRequierment] = source.[StrenghtRequierment],
        target.[StealthDisadvantage] = source.[StealthDisadvantage],
        target.[DeletedAt] = NULL
WHEN NOT MATCHED THEN
    INSERT ([EquipmentId], [Discriminator], [Name], [Type], [Cost], [Weight], [DamageAmount], [DamageType], [WeaponProperties], [ArmourClass], [StrenghtRequierment], [StealthDisadvantage], [DeletedAt])
    VALUES (source.[EquipmentId], source.[Discriminator], source.[Name], source.[Type], source.[Cost], source.[Weight], source.[DamageAmount], source.[DamageType], source.[WeaponProperties], source.[ArmourClass], source.[StrenghtRequierment], source.[StealthDisadvantage], NULL);

IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'[Equipment]') AND [name] = 'EquipmentId')
    SET IDENTITY_INSERT [Equipment] OFF;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "CharacterLevels",
                keyColumn: "LevelId",
                keyValue: 20);

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
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1101);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1102);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1103);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1104);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1105);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1106);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1107);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1108);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1109);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1110);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1111);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1112);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1113);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1201);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1202);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1203);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1204);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1205);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1206);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1207);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1208);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1209);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1210);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1211);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1212);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1213);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1214);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1215);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1216);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1217);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1301);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1302);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1303);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1304);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1305);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1306);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1307);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1308);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1309);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1310);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1311);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1312);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1313);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1314);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1315);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1316);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1317);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1318);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1319);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1320);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1321);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1322);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1323);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1324);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1325);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1326);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1327);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1328);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1329);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1330);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1331);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1332);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1333);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1334);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1335);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1336);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1337);

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

            migrationBuilder.UpdateData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1001,
                columns: new[] { "Cost", "DamageAmount", "DamageType", "Name", "Type", "Weight" },
                values: new object[] { 15, "1d8", "Slashing", "Longsword", "Weapon", 3 });

            migrationBuilder.UpdateData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1004,
                columns: new[] { "Cost", "DamageAmount", "DamageType", "Name", "Type" },
                values: new object[] { 25, "1d8", "Piercing", "Rapier", "Weapon" });

            migrationBuilder.UpdateData(
                table: "Equipment",
                keyColumn: "EquipmentId",
                keyValue: 1025,
                columns: new[] { "Name", "Type" },
                values: new object[] { "Dagger", "Weapon" });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "EquipmentId", "ArmourClass", "Cost", "DeletedAt", "Discriminator", "Name", "StealthDisadvantage", "StrenghtRequierment", "Type", "Weight" },
                values: new object[] { 1002, 0, 75, null, "Armour", "Chain Mail", false, 0, "Armour", 55 });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "EquipmentId", "Cost", "DeletedAt", "Discriminator", "Name", "Type", "Weight" },
                values: new object[,]
                {
                    { 1003, 2, null, "AdventuringGear", "Backpack", "Gear", 5 },
                    { 1015, 25, null, "AdventuringGear", "Thieves' Tools", "Gear", 1 }
                });
        }
    }
}
