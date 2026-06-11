using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DnDDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("DnD_Character_Sheet_Creator.DAL")));

builder.Services.AddControllersWithViews();

builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<DnDDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
    options.LogoutPath = "/Account/SignOut";
    options.AccessDeniedPath = "/Account/SignIn";
    options.Cookie.Name = "AdventurerLedger.Auth";
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-google";
    });

builder.Services.AddAuthorization();

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

    await SeedIdentityAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static async Task SeedIdentityAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in new[] { "Admin", "Manager" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var players = context.Players.Where(player => player.DeletedAt == null).ToList();
    foreach (var player in players)
    {
        var user = await userManager.FindByNameAsync(player.Username);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = player.Username,
                Email = player.Email,
                PlayerId = player.PlayerId,
                OIB = player.PlayerId.ToString().PadLeft(11, '0'),
                JMBG = player.PlayerId.ToString().PadLeft(13, '0')
            };

            var createResult = await userManager.CreateAsync(user, string.IsNullOrWhiteSpace(player.Password) ? "changeme" : player.Password);
            if (!createResult.Succeeded)
            {
                continue;
            }
        }

        var roleName = player.IsAdmin ? "Admin" : "Manager";
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}

public partial class Program { }
