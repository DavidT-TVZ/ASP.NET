using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DnDDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("DnD_Character_Sheet_Creator.DAL")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPlayerRepository, EFPlayerRepository>();
builder.Services.AddScoped<ICharacterRepository, EFCharacterRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
    context.Database.Migrate();
    // Seed database from mock repositories if empty
    if (!context.Players.Any())
    {
        var players = MockRepositorySeed.CreatePlayers();
        var charactersByPlayer = MockRepositorySeed.CreateCharactersByPlayerId();

        foreach (var player in players)
        {
            // preserve original id for lookup, then clear identity so SQL Server assigns one
            var originalPlayerId = player.PlayerId;
            player.PlayerId = 0;

            if (charactersByPlayer.TryGetValue(originalPlayerId, out var chars))
            {
                foreach (var ch in chars)
                {
                    ch.CharacterId = 0;
                    if (ch.Level != null)
                    {
                        ch.Level.LevelId = 0;
                    }

                    foreach (var eq in ch.EquipmentList)
                    {
                        eq.EquipmentId = 0;
                    }

                    // ensure FK will be set by relationship
                    ch.Player = player;
                    player.CharacterList.Add(ch);
                }
            }
        }

        context.Players.AddRange(players);
        context.SaveChanges();
    }
}

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

app.Run();
