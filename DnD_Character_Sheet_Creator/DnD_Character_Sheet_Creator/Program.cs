using DnD_Character_Sheet_Creator.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPlayerRepository, MockPlayerRepository>();
builder.Services.AddSingleton<ICharacterRepository, MockCharacterRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

/*
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
*/

app.Run();
