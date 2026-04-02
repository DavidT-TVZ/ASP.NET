using System;
using System.Collections.Generic;
using DnD_Character_Sheet_Creator;
using DnD_Character_Sheet_Creator.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Create sample players and register as a singleton so controllers can use them.
var players = new List<Player>
{
    new Player
    {
        PlayerId = 1,
        Name = "John",
        Surname = "Smith",
        Username = "john.smith",
        Email = "john.smith@example.com",
        Password = "changeme",
        LastLogin = DateTime.UtcNow,
        CharacterList = new List<Character>
        {
            new Character
            {
                CharacterId = 1,
                CharacterName = "Eldin Reed",
                Race = RaceEnum.Human,
                Background = BackgroundEnum.Sage,
                Alignment = AlignmentEnum.NeutralGood,
                Class = ClassEnum.Wizard,
                Level = new CharacterLevel { LevelId = 1, Level = 3, CurrentExperiencePoints = 900, ExperiencePointsToNextLevel = 2700, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-2) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 1, Type = "Weapon", Name = "Longsword", Cost = 15, Weight = 3, DamageAmount = "1d8", DamageType = "Slashing", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Versatile } },
                    new Armour { EquipmentId = 2, Type = "Armour", Name = "Chain Mail", Cost = 75, Weight = 55, ArmourClass = 16, StrenghtRequierment = 13, StealthDisadvantage = true },
                    new AdventuringGear { EquipmentId = 3, Type = "Gear", Name = "Backpack", Cost = 2, Weight = 5 }
                }
            },
            new Character
            {
                CharacterId = 2,
                CharacterName = "Sierra Vale",
                Race = RaceEnum.Half_Elf,
                Background = BackgroundEnum.Entertainer,
                Alignment = AlignmentEnum.ChaoticGood,
                Class = ClassEnum.Bard,
                Level = new CharacterLevel { LevelId = 2, Level = 2, CurrentExperiencePoints = 300, ExperiencePointsToNextLevel = 600, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-1) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 4, Type = "Weapon", Name = "Rapier", Cost = 25, Weight = 2, DamageAmount = "1d8", DamageType = "Piercing", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Finesse } },
                    new Armour { EquipmentId = 5, Type = "Armour", Name = "Leather", Cost = 10, Weight = 10, ArmourClass = 11, StrenghtRequierment = 0, StealthDisadvantage = false },
                    new AdventuringGear { EquipmentId = 6, Type = "Gear", Name = "Lute", Cost = 35, Weight = 3 }
                }
            },
            new Character
            {
                CharacterId = 3,
                CharacterName = "Borin Ironfist",
                Race = RaceEnum.Dwarf,
                Background = BackgroundEnum.Soldier,
                Alignment = AlignmentEnum.LawfulNeutral,
                Class = ClassEnum.Fighter,
                Level = new CharacterLevel { LevelId = 3, Level = 4, CurrentExperiencePoints = 2700, ExperiencePointsToNextLevel = 6500, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-6) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 7, Type = "Weapon", Name = "Battleaxe", Cost = 10, Weight = 4, DamageAmount = "1d8", DamageType = "Slashing", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Versatile } },
                    new Armour { EquipmentId = 8, Type = "Armour", Name = "Scale Mail", Cost = 50, Weight = 45, ArmourClass = 14, StrenghtRequierment = 0, StealthDisadvantage = true },
                    new AdventuringGear { EquipmentId = 9, Type = "Gear", Name = "Climber's Kit", Cost = 25, Weight = 12 }
                }
            }
        }
    },
    new Player
    {
        PlayerId = 2,
        Name = "Maria",
        Surname = "Garcia",
        Username = "maria.garcia",
        Email = "maria.garcia@example.com",
        Password = "changeme",
        LastLogin = DateTime.UtcNow.AddDays(-1),
        CharacterList = new List<Character>
        {
            new Character
            {
                CharacterId = 4,
                CharacterName = "Lucia Morales",
                Race = RaceEnum.Human,
                Background = BackgroundEnum.Noble,
                Alignment = AlignmentEnum.LawfulGood,
                Class = ClassEnum.Paladin,
                Level = new CharacterLevel { LevelId = 4, Level = 5, CurrentExperiencePoints = 6500, ExperiencePointsToNextLevel = 14000, ProficiencyBonus = 3, DateOfLastLevelUp = DateTime.UtcNow.AddYears(-1) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 10, Type = "Weapon", Name = "Warhammer", Cost = 15, Weight = 2, DamageAmount = "1d8", DamageType = "Bludgeoning", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Versatile } },
                    new Armour { EquipmentId = 11, Type = "Armour", Name = "Plate", Cost = 1500, Weight = 65, ArmourClass = 18, StrenghtRequierment = 15, StealthDisadvantage = true },
                    new AdventuringGear { EquipmentId = 12, Type = "Gear", Name = "Holy Symbol", Cost = 5, Weight = 1 }
                }
            },
            new Character
            {
                CharacterId = 5,
                CharacterName = "Rafael Ortega",
                Race = RaceEnum.Human,
                Background = BackgroundEnum.Criminal,
                Alignment = AlignmentEnum.Neutral,
                Class = ClassEnum.Rogue,
                Level = new CharacterLevel { LevelId = 5, Level = 3, CurrentExperiencePoints = 900, ExperiencePointsToNextLevel = 2700, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-3) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 13, Type = "Weapon", Name = "Shortsword", Cost = 10, Weight = 2, DamageAmount = "1d6", DamageType = "Piercing", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Finesse, WeaponPropertiesEnum.Light } },
                    new Armour { EquipmentId = 14, Type = "Armour", Name = "Studded Leather", Cost = 45, Weight = 13, ArmourClass = 12, StrenghtRequierment = 0, StealthDisadvantage = false },
                    new AdventuringGear { EquipmentId = 15, Type = "Gear", Name = "Thieves' Tools", Cost = 25, Weight = 1 }
                }
            },
            new Character
            {
                CharacterId = 6,
                CharacterName = "Marta Ruiz",
                Race = RaceEnum.Halfling,
                Background = BackgroundEnum.Folk_Hero,
                Alignment = AlignmentEnum.NeutralGood,
                Class = ClassEnum.Ranger,
                Level = new CharacterLevel { LevelId = 6, Level = 2, CurrentExperiencePoints = 300, ExperiencePointsToNextLevel = 600, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-2) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 16, Type = "Weapon", Name = "Shortbow", Cost = 25, Weight = 2, DamageAmount = "1d6", DamageType = "Piercing", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Ammunition, WeaponPropertiesEnum.Range } },
                    new Armour { EquipmentId = 17, Type = "Armour", Name = "Padded", Cost = 5, Weight = 8, ArmourClass = 11, StrenghtRequierment = 0, StealthDisadvantage = false },
                    new AdventuringGear { EquipmentId = 18, Type = "Gear", Name = "Bedroll", Cost = 1, Weight = 7 }
                }
            }
        }
    },
    new Player
    {
        PlayerId = 3,
        Name = "Michael",
        Surname = "Brown",
        Username = "michael.brown",
        Email = "michael.brown@example.com",
        Password = "changeme",
        LastLogin = DateTime.UtcNow.AddHours(-5),
        CharacterList = new List<Character>
        {
            new Character
            {
                CharacterId = 7,
                CharacterName = "Daniel Carter",
                Race = RaceEnum.Human,
                Background = BackgroundEnum.Guild_Artisan,
                Alignment = AlignmentEnum.NeutralGood,
                Class = ClassEnum.Cleric,
                Level = new CharacterLevel { LevelId = 7, Level = 4, CurrentExperiencePoints = 2700, ExperiencePointsToNextLevel = 6500, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-5) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 19, Type = "Weapon", Name = "Mace", Cost = 5, Weight = 4, DamageAmount = "1d6", DamageType = "Bludgeoning", WeaponProperties = new List<WeaponPropertiesEnum> { } },
                    new Armour { EquipmentId = 20, Type = "Armour", Name = "Scale Mail", Cost = 50, Weight = 45, ArmourClass = 14, StrenghtRequierment = 0, StealthDisadvantage = true },
                    new AdventuringGear { EquipmentId = 21, Type = "Gear", Name = "Artisan's Tools", Cost = 5, Weight = 8 }
                }
            },
            new Character
            {
                CharacterId = 8,
                CharacterName = "Hannah Lee",
                Race = RaceEnum.Elf,
                Background = BackgroundEnum.Sage,
                Alignment = AlignmentEnum.Neutral,
                Class = ClassEnum.Druid,
                Level = new CharacterLevel { LevelId = 8, Level = 2, CurrentExperiencePoints = 300, ExperiencePointsToNextLevel = 600, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-1) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 22, Type = "Weapon", Name = "Quarterstaff", Cost = 0, Weight = 4, DamageAmount = "1d6", DamageType = "Bludgeoning", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Versatile } },
                    new Armour { EquipmentId = 23, Type = "Armour", Name = "Hide", Cost = 10, Weight = 12, ArmourClass = 12, StrenghtRequierment = 0, StealthDisadvantage = false },
                    new AdventuringGear { EquipmentId = 24, Type = "Gear", Name = "Herbalism Kit", Cost = 5, Weight = 3 }
                }
            },
            new Character
            {
                CharacterId = 9,
                CharacterName = "Michael Drake",
                Race = RaceEnum.Tiefling,
                Background = BackgroundEnum.Sailor,
                Alignment = AlignmentEnum.ChaoticNeutral,
                Class = ClassEnum.Sorcerer,
                Level = new CharacterLevel { LevelId = 9, Level = 3, CurrentExperiencePoints = 900, ExperiencePointsToNextLevel = 2700, ProficiencyBonus = 2, DateOfLastLevelUp = DateTime.UtcNow.AddMonths(-4) },
                EquipmentList = new List<Equipment>
                {
                    new Weapon { EquipmentId = 25, Type = "Weapon", Name = "Dagger", Cost = 2, Weight = 1, DamageAmount = "1d4", DamageType = "Piercing", WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Finesse, WeaponPropertiesEnum.Thrown } },
                    new Armour { EquipmentId = 26, Type = "Armour", Name = "Robes", Cost = 1, Weight = 2, ArmourClass = 10, StrenghtRequierment = 0, StealthDisadvantage = false },
                    new AdventuringGear { EquipmentId = 27, Type = "Gear", Name = "Navigator's Tools", Cost = 25, Weight = 2 }
                }
            }
        }
    }
};

builder.Services.AddSingleton(players);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

var mostRecentPlayer = players
    .OrderByDescending(p => p.LastLogin)
    .FirstOrDefault();

var playersWithNoCharacters = players
    .Where(p => p.CharacterList == null || !p.CharacterList.Any())
    .ToList();

var highLevelCharacters = players
    .SelectMany(p => p.CharacterList)
    .Where(c => c.Level != null && c.Level.Level > 3)
    .ToList();

var top3ByCharacterCount = players
    .OrderByDescending(p => p.CharacterList?.Count ?? 0)
    .Take(3)
    .ToList();

var distinctRaces = players
    .SelectMany(p => p.CharacterList)
    .Select(c => c.Race)
    .Distinct()
    .ToList();

var weightPerCharacter = players
    .SelectMany(p => p.CharacterList)
    .Select(c => new
    {
        c.CharacterId,
        c.CharacterName,
        TotalWeight = c.EquipmentList?.Sum(eq => eq.Weight) ?? 0
    })
    .ToList();

var charactersByClass = players
    .SelectMany(p => p.CharacterList)
    .GroupBy(c => c.Class)
    .Select(g => new { Class = g.Key, Characters = g.ToList() })
    .ToList();

// Small console output for the LINQ queries
Console.WriteLine();
Console.WriteLine("Most recent player login:");
if (mostRecentPlayer != null)
{
    Console.WriteLine($"- {mostRecentPlayer.Username} (LastLogin: {mostRecentPlayer.LastLogin:u})");
}
else
{
    Console.WriteLine("- (none)");
}

Console.WriteLine();
Console.WriteLine("Players with no characters:");
if (playersWithNoCharacters.Any())
{
    foreach (var p in playersWithNoCharacters)
    {
        Console.WriteLine($"- {p.Username} ({p.Name} {p.Surname})");
    }
}
else
{
    Console.WriteLine("- (none)");
}

Console.WriteLine();
Console.WriteLine("Characters with level > 3:");
if (highLevelCharacters.Any())
{
    foreach (var c in highLevelCharacters)
    {
        Console.WriteLine($"- {c.CharacterName} (Level {c.Level.Level})");
    }
}
else
{
    Console.WriteLine("- (none)");
}

Console.WriteLine();
Console.WriteLine("Top 3 players by character count:");
foreach (var p in top3ByCharacterCount)
{
    Console.WriteLine($"- {p.Username}: {p.CharacterList?.Count ?? 0} characters");
}

Console.WriteLine();
Console.WriteLine("Distinct races used by characters:");
Console.WriteLine($"- {string.Join(", ", distinctRaces)}");

Console.WriteLine();
Console.WriteLine("Total equipment weight per character:");
foreach (var w in weightPerCharacter)
{
    Console.WriteLine($"- {w.CharacterName}: {w.TotalWeight} lb");
}

Console.WriteLine();
Console.WriteLine("Characters grouped by class:");
foreach (var g in charactersByClass)
{
    Console.WriteLine($"- {g.Class}: {g.Characters.Count} character(s)");
}
Console.WriteLine();

app.Run();
