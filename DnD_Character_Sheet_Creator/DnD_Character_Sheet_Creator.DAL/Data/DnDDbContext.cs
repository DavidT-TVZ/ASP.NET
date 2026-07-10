using DnD_Character_Sheet_Creator.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DnD_Character_Sheet_Creator.Data
{
    public class DnDDbContext : IdentityDbContext<AppUser>
    {
        public DnDDbContext(DbContextOptions<DnDDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterLevel> CharacterLevels { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Equipment> Equipment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
            // Suppress the pending model changes warning for design-time migration execution
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Surname).IsRequired();
                entity.Property(p => p.Username).IsRequired();
                entity.Property(p => p.Email).IsRequired();
                entity.Property(p => p.Password).IsRequired();

                entity.HasMany(p => p.CharacterList)
                    .WithOne(c => c.Player)
                    .HasForeignKey(c => c.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(c => c.CharacterId);
                entity.Property(c => c.CharacterName).IsRequired();
                entity.Property(c => c.PlayerId).IsRequired();

                entity.HasOne(c => c.Level)
                    .WithOne()
                    .HasForeignKey<Character>(c => c.LevelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.EquipmentList)
                    .WithMany(e => e.Characters)
                    .UsingEntity<Dictionary<string, object>>(
                        "CharacterEquipment",
                        right => right.HasOne<Equipment>()
                            .WithMany()
                            .HasForeignKey("EquipmentId")
                            .OnDelete(DeleteBehavior.Cascade),
                        left => left.HasOne<Character>()
                            .WithMany()
                            .HasForeignKey("CharacterId")
                            .OnDelete(DeleteBehavior.Cascade),
                        join => join.HasKey("CharacterId", "EquipmentId"));

                entity.HasMany(c => c.Attachments)
                    .WithOne(a => a.Character)
                    .HasForeignKey(a => a.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CharacterLevel>(entity =>
            {
                entity.HasKey(cl => cl.LevelId);
                // Seed canonical D&D 5e level progression (PHB) — levels 1-20
                entity.HasData(
                    new Models.CharacterLevel { LevelId = 1, Level = 1, CurrentExperiencePoints = 0, ExperiencePointsToNextLevel = 300, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 2, Level = 2, CurrentExperiencePoints = 300, ExperiencePointsToNextLevel = 900, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 3, Level = 3, CurrentExperiencePoints = 900, ExperiencePointsToNextLevel = 2700, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 4, Level = 4, CurrentExperiencePoints = 2700, ExperiencePointsToNextLevel = 6500, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 5, Level = 5, CurrentExperiencePoints = 6500, ExperiencePointsToNextLevel = 14000, ProficiencyBonus = 3, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 6, Level = 6, CurrentExperiencePoints = 14000, ExperiencePointsToNextLevel = 23000, ProficiencyBonus = 3, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 7, Level = 7, CurrentExperiencePoints = 23000, ExperiencePointsToNextLevel = 34000, ProficiencyBonus = 3, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 8, Level = 8, CurrentExperiencePoints = 34000, ExperiencePointsToNextLevel = 48000, ProficiencyBonus = 3, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 9, Level = 9, CurrentExperiencePoints = 48000, ExperiencePointsToNextLevel = 64000, ProficiencyBonus = 4, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 10, Level = 10, CurrentExperiencePoints = 64000, ExperiencePointsToNextLevel = 85000, ProficiencyBonus = 4, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 11, Level = 11, CurrentExperiencePoints = 85000, ExperiencePointsToNextLevel = 100000, ProficiencyBonus = 4, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 12, Level = 12, CurrentExperiencePoints = 100000, ExperiencePointsToNextLevel = 120000, ProficiencyBonus = 4, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 13, Level = 13, CurrentExperiencePoints = 120000, ExperiencePointsToNextLevel = 140000, ProficiencyBonus = 5, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 14, Level = 14, CurrentExperiencePoints = 140000, ExperiencePointsToNextLevel = 165000, ProficiencyBonus = 5, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 15, Level = 15, CurrentExperiencePoints = 165000, ExperiencePointsToNextLevel = 195000, ProficiencyBonus = 5, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 16, Level = 16, CurrentExperiencePoints = 195000, ExperiencePointsToNextLevel = 225000, ProficiencyBonus = 5, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 17, Level = 17, CurrentExperiencePoints = 225000, ExperiencePointsToNextLevel = 265000, ProficiencyBonus = 6, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 18, Level = 18, CurrentExperiencePoints = 265000, ExperiencePointsToNextLevel = 305000, ProficiencyBonus = 6, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 19, Level = 19, CurrentExperiencePoints = 305000, ExperiencePointsToNextLevel = 355000, ProficiencyBonus = 6, DateOfLastLevelUp = DateTime.UtcNow },
                    new Models.CharacterLevel { LevelId = 20, Level = 20, CurrentExperiencePoints = 355000, ExperiencePointsToNextLevel = 0, ProficiencyBonus = 6, DateOfLastLevelUp = DateTime.UtcNow }
                );
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasKey(a => a.AttachmentId);
                entity.Property(a => a.FileName).IsRequired();
                entity.Property(a => a.FilePath).IsRequired();
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EquipmentId);
                entity.Property(e => e.Name).IsRequired();
                
            });

            // Seed derived equipment types using their concrete entity types (required for EF Core HasData)
            modelBuilder.Entity<Weapon>().HasData(
                new Models.Weapon { EquipmentId = 1001, Type = "Simple Melee Weapon", Name = "Club", Cost = 0, Weight = 2, DamageAmount = "1d4", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1002, Type = "Simple Melee Weapon", Name = "Dagger", Cost = 2, Weight = 1, DamageAmount = "1d4", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1003, Type = "Simple Melee Weapon", Name = "Greatclub", Cost = 0, Weight = 10, DamageAmount = "1d8", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1004, Type = "Simple Melee Weapon", Name = "Handaxe", Cost = 5, Weight = 2, DamageAmount = "1d6", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1005, Type = "Simple Melee Weapon", Name = "Javelin", Cost = 5, Weight = 2, DamageAmount = "1d6", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1006, Type = "Simple Melee Weapon", Name = "Light Hammer", Cost = 2, Weight = 2, DamageAmount = "1d4", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1007, Type = "Simple Melee Weapon", Name = "Mace", Cost = 5, Weight = 4, DamageAmount = "1d6", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1008, Type = "Simple Melee Weapon", Name = "Quarterstaff", Cost = 0, Weight = 4, DamageAmount = "1d6", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1009, Type = "Simple Melee Weapon", Name = "Sickle", Cost = 1, Weight = 2, DamageAmount = "1d4", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1010, Type = "Simple Melee Weapon", Name = "Spear", Cost = 1, Weight = 3, DamageAmount = "1d6", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1011, Type = "Simple Ranged Weapon", Name = "Light Crossbow", Cost = 25, Weight = 5, DamageAmount = "1d8", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1012, Type = "Simple Ranged Weapon", Name = "Shortbow", Cost = 25, Weight = 2, DamageAmount = "1d6", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1013, Type = "Martial Melee Weapon", Name = "Longsword", Cost = 15, Weight = 3, DamageAmount = "1d8", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1014, Type = "Martial Melee Weapon", Name = "Shortsword", Cost = 10, Weight = 2, DamageAmount = "1d6", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1015, Type = "Martial Melee Weapon", Name = "Scimitar", Cost = 25, Weight = 3, DamageAmount = "1d6", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1016, Type = "Martial Melee Weapon", Name = "Warhammer", Cost = 15, Weight = 5, DamageAmount = "1d8", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1017, Type = "Martial Melee Weapon", Name = "Battleaxe", Cost = 10, Weight = 4, DamageAmount = "1d8", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1018, Type = "Martial Melee Weapon", Name = "Greatsword", Cost = 50, Weight = 6, DamageAmount = "2d6", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1019, Type = "Martial Melee Weapon", Name = "Halberd", Cost = 20, Weight = 6, DamageAmount = "1d10", DamageType = "Slashing" },
                new Models.Weapon { EquipmentId = 1020, Type = "Martial Melee Weapon", Name = "Pike", Cost = 5, Weight = 18, DamageAmount = "1d10", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1021, Type = "Martial Melee Weapon", Name = "Maul", Cost = 10, Weight = 10, DamageAmount = "2d6", DamageType = "Bludgeoning" },
                new Models.Weapon { EquipmentId = 1022, Type = "Martial Melee Weapon", Name = "Rapier", Cost = 25, Weight = 2, DamageAmount = "1d8", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1023, Type = "Martial Ranged Weapon", Name = "Longbow", Cost = 50, Weight = 2, DamageAmount = "1d8", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1024, Type = "Martial Ranged Weapon", Name = "Heavy Crossbow", Cost = 50, Weight = 18, DamageAmount = "1d10", DamageType = "Piercing" },
                new Models.Weapon { EquipmentId = 1025, Type = "Simple Melee Weapon", Name = "Throwing Dagger", Cost = 2, Weight = 1, DamageAmount = "1d4", DamageType = "Piercing" }
            );

            modelBuilder.Entity<Armour>().HasData(
                new Models.Armour { EquipmentId = 1101, Type = "Light Armour", Name = "Padded", Cost = 5, Weight = 8, ArmourClass = 11, StealthDisadvantage = false, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1102, Type = "Light Armour", Name = "Leather", Cost = 10, Weight = 10, ArmourClass = 11, StealthDisadvantage = false, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1103, Type = "Light Armour", Name = "Studded Leather", Cost = 45, Weight = 13, ArmourClass = 12, StealthDisadvantage = false, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1104, Type = "Medium Armour", Name = "Hide", Cost = 10, Weight = 12, ArmourClass = 12, StealthDisadvantage = false, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1105, Type = "Medium Armour", Name = "Chain Shirt", Cost = 50, Weight = 20, ArmourClass = 13, StealthDisadvantage = false, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1106, Type = "Medium Armour", Name = "Scale Mail", Cost = 50, Weight = 45, ArmourClass = 14, StealthDisadvantage = true, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1107, Type = "Medium Armour", Name = "Breastplate", Cost = 400, Weight = 20, ArmourClass = 14, StealthDisadvantage = false, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1108, Type = "Medium Armour", Name = "Half Plate", Cost = 750, Weight = 40, ArmourClass = 15, StealthDisadvantage = true, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1109, Type = "Heavy Armour", Name = "Ring Mail", Cost = 30, Weight = 40, ArmourClass = 14, StealthDisadvantage = true, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1110, Type = "Heavy Armour", Name = "Chain Mail", Cost = 75, Weight = 55, ArmourClass = 16, StealthDisadvantage = true, StrenghtRequierment = 13 },
                new Models.Armour { EquipmentId = 1111, Type = "Heavy Armour", Name = "Splint", Cost = 200, Weight = 60, ArmourClass = 17, StealthDisadvantage = true, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1112, Type = "Heavy Armour", Name = "Plate", Cost = 1500, Weight = 65, ArmourClass = 18, StealthDisadvantage = true, StrenghtRequierment = 0 },
                new Models.Armour { EquipmentId = 1113, Type = "Shield", Name = "Shield", Cost = 10, Weight = 6, ArmourClass = 2, StealthDisadvantage = false, StrenghtRequierment = 0 }
            );

            modelBuilder.Entity<AdventuringGear>().HasData(
                new Models.AdventuringGear { EquipmentId = 1201, Type = "Gear", Name = "Backpack", Cost = 2, Weight = 5 },
                new Models.AdventuringGear { EquipmentId = 1202, Type = "Gear", Name = "Bedroll", Cost = 1, Weight = 7 },
                new Models.AdventuringGear { EquipmentId = 1203, Type = "Gear", Name = "Mess Kit", Cost = 2, Weight = 1 },
                new Models.AdventuringGear { EquipmentId = 1204, Type = "Gear", Name = "Rations (1 day)", Cost = 5, Weight = 2 },
                new Models.AdventuringGear { EquipmentId = 1205, Type = "Gear", Name = "Waterskin", Cost = 2, Weight = 5 },
                new Models.AdventuringGear { EquipmentId = 1206, Type = "Gear", Name = "Hempen Rope (50 ft)", Cost = 1, Weight = 10 },
                new Models.AdventuringGear { EquipmentId = 1207, Type = "Gear", Name = "Crowbar", Cost = 2, Weight = 5 },
                new Models.AdventuringGear { EquipmentId = 1208, Type = "Gear", Name = "Hammer", Cost = 1, Weight = 3 },
                new Models.AdventuringGear { EquipmentId = 1209, Type = "Gear", Name = "Pitons (10)", Cost = 5, Weight = 2 },
                new Models.AdventuringGear { EquipmentId = 1210, Type = "Gear", Name = "Tinderbox", Cost = 5, Weight = 1 },
                new Models.AdventuringGear { EquipmentId = 1211, Type = "Gear", Name = "Torches (10)", Cost = 1, Weight = 1 },
                new Models.AdventuringGear { EquipmentId = 1212, Type = "Gear", Name = "Hooded Lantern", Cost = 5, Weight = 2 },
                new Models.AdventuringGear { EquipmentId = 1213, Type = "Gear", Name = "Grappling Hook", Cost = 2, Weight = 4 },
                new Models.AdventuringGear { EquipmentId = 1214, Type = "Gear", Name = "Mirror (steel)", Cost = 5, Weight = 0 },
                new Models.AdventuringGear { EquipmentId = 1215, Type = "Gear", Name = "Chalk (1 piece)", Cost = 0, Weight = 0 },
                new Models.AdventuringGear { EquipmentId = 1216, Type = "Gear", Name = "Lantern (bullseye)", Cost = 10, Weight = 3 },
                new Models.AdventuringGear { EquipmentId = 1217, Type = "Gear", Name = "Candles (10)", Cost = 1, Weight = 0 }
            );

            modelBuilder.Entity<Tools>().HasData(
                new Models.Tools { EquipmentId = 1301, Type = "Artisan's Tools", Name = "Alchemist's Supplies", Cost = 50, Weight = 8 },
                new Models.Tools { EquipmentId = 1302, Type = "Artisan's Tools", Name = "Brewer's Supplies", Cost = 20, Weight = 9 },
                new Models.Tools { EquipmentId = 1303, Type = "Artisan's Tools", Name = "Calligrapher's Supplies", Cost = 10, Weight = 5 },
                new Models.Tools { EquipmentId = 1304, Type = "Artisan's Tools", Name = "Carpenter's Tools", Cost = 8, Weight = 6 },
                new Models.Tools { EquipmentId = 1305, Type = "Artisan's Tools", Name = "Cartographer's Tools", Cost = 15, Weight = 6 },
                new Models.Tools { EquipmentId = 1306, Type = "Artisan's Tools", Name = "Cobbler's Tools", Cost = 5, Weight = 5 },
                new Models.Tools { EquipmentId = 1307, Type = "Artisan's Tools", Name = "Cook's Utensils", Cost = 1, Weight = 8 },
                new Models.Tools { EquipmentId = 1308, Type = "Artisan's Tools", Name = "Glassblower's Tools", Cost = 30, Weight = 5 },
                new Models.Tools { EquipmentId = 1309, Type = "Artisan's Tools", Name = "Jeweler's Tools", Cost = 25, Weight = 2 },
                new Models.Tools { EquipmentId = 1310, Type = "Artisan's Tools", Name = "Leatherworker's Tools", Cost = 5, Weight = 5 },
                new Models.Tools { EquipmentId = 1311, Type = "Artisan's Tools", Name = "Mason's Tools", Cost = 10, Weight = 8 },
                new Models.Tools { EquipmentId = 1312, Type = "Artisan's Tools", Name = "Painter's Supplies", Cost = 10, Weight = 5 },
                new Models.Tools { EquipmentId = 1313, Type = "Artisan's Tools", Name = "Potter's Tools", Cost = 10, Weight = 3 },
                new Models.Tools { EquipmentId = 1314, Type = "Artisan's Tools", Name = "Smith's Tools", Cost = 20, Weight = 8 },
                new Models.Tools { EquipmentId = 1315, Type = "Artisan's Tools", Name = "Tinker's Tools", Cost = 50, Weight = 10 },
                new Models.Tools { EquipmentId = 1316, Type = "Artisan's Tools", Name = "Weaver's Tools", Cost = 1, Weight = 5 },
                new Models.Tools { EquipmentId = 1317, Type = "Artisan's Tools", Name = "Woodcarver's Tools", Cost = 1, Weight = 5 },
                new Models.Tools { EquipmentId = 1318, Type = "Gaming Set", Name = "Dice Set", Cost = 1, Weight = 0 },
                new Models.Tools { EquipmentId = 1319, Type = "Gaming Set", Name = "Dragonchess Set", Cost = 1, Weight = 1 },
                new Models.Tools { EquipmentId = 1320, Type = "Gaming Set", Name = "Playing Card Set", Cost = 5, Weight = 0 },
                new Models.Tools { EquipmentId = 1321, Type = "Gaming Set", Name = "Three-Dragon Ante Set", Cost = 1, Weight = 0 },
                new Models.Tools { EquipmentId = 1322, Type = "Musical Instrument", Name = "Bagpipes", Cost = 30, Weight = 6 },
                new Models.Tools { EquipmentId = 1323, Type = "Musical Instrument", Name = "Drum", Cost = 6, Weight = 3 },
                new Models.Tools { EquipmentId = 1324, Type = "Musical Instrument", Name = "Dulcimer", Cost = 25, Weight = 10 },
                new Models.Tools { EquipmentId = 1325, Type = "Musical Instrument", Name = "Flute", Cost = 2, Weight = 1 },
                new Models.Tools { EquipmentId = 1326, Type = "Musical Instrument", Name = "Lute", Cost = 35, Weight = 2 },
                new Models.Tools { EquipmentId = 1327, Type = "Musical Instrument", Name = "Lyre", Cost = 30, Weight = 2 },
                new Models.Tools { EquipmentId = 1328, Type = "Musical Instrument", Name = "Horn", Cost = 3, Weight = 2 },
                new Models.Tools { EquipmentId = 1329, Type = "Musical Instrument", Name = "Pan Flute", Cost = 12, Weight = 2 },
                new Models.Tools { EquipmentId = 1330, Type = "Musical Instrument", Name = "Shawm", Cost = 2, Weight = 1 },
                new Models.Tools { EquipmentId = 1331, Type = "Musical Instrument", Name = "Viol", Cost = 30, Weight = 1 },
                new Models.Tools { EquipmentId = 1332, Type = "Tool Kit", Name = "Disguise Kit", Cost = 25, Weight = 3 },
                new Models.Tools { EquipmentId = 1333, Type = "Tool Kit", Name = "Forgery Kit", Cost = 15, Weight = 5 },
                new Models.Tools { EquipmentId = 1334, Type = "Tool Kit", Name = "Herbalism Kit", Cost = 5, Weight = 3 },
                new Models.Tools { EquipmentId = 1335, Type = "Tool Kit", Name = "Navigator's Tools", Cost = 25, Weight = 2 },
                new Models.Tools { EquipmentId = 1336, Type = "Tool Kit", Name = "Poisoner's Kit", Cost = 50, Weight = 2 },
                new Models.Tools { EquipmentId = 1337, Type = "Tool Kit", Name = "Thieves' Tools", Cost = 25, Weight = 1 }
            );
        }
    }
}
