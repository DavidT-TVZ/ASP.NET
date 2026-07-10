using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<DnDDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.MigrationsAssembly("DnD_Character_Sheet_Creator.DAL")));
}
else
{
    builder.Services.AddDbContext<DnDDbContext>(options =>
    {
        options.UseInMemoryDatabase("DnDTestDb");
        options.UseLazyLoadingProxies();
    });
}

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

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
            options.CallbackPath = "/signin-google";
        });
}
else
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
        options.DefaultChallengeScheme = TestAuthHandler.Scheme;
    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
}

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPlayerRepository, EFPlayerRepository>();
builder.Services.AddScoped<ICharacterRepository, EFCharacterRepository>();

var app = builder.Build();

var startupLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
startupLogger.LogInformation("Application booting in {Environment}", app.Environment.EnvironmentName);

if (app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
        context.Database.EnsureCreated();

        if (!context.Players.Any())
        {
            var players = MockRepositorySeed.CreatePlayers();
            var charactersByPlayer = MockRepositorySeed.CreateCharactersByPlayerId();

            foreach (var player in players)
            {
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
}
else
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            startupLogger.LogInformation("Applying database migrations and seed data");
            var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
            context.Database.Migrate();
            // Seed database from mock repositories if empty
            if (!context.Players.Any())
            {
                startupLogger.LogInformation("Database empty, seeding mock players and characters");
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
            startupLogger.LogInformation("Startup migration and identity seed completed");
        }
    }
    catch (InvalidOperationException ex)
    {
        startupLogger.LogWarning(ex, "Skipping DB migration/seed during startup");
    }
}

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseAuthentication();

if (app.Environment.IsEnvironment("Testing"))
{
    app.Use(async (context, next) =>
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "john.smith"),
            new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString()),
            new Claim("FullName", "John Smith")
        };

        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestRequest", ClaimTypes.Name, ClaimTypes.Role));
        await next();
    });
}

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

    foreach (var role in new[] { "Admin", "Manager", "User" })
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

        await SyncManagedRoleAsync(userManager, user, player.Role);
    }
}

static async Task SyncManagedRoleAsync(UserManager<AppUser> userManager, AppUser user, RoleEnum targetRoleEnum)
{
    var managedRoles = new[] { "Admin", "Manager", "User" };
    var targetRole = targetRoleEnum.ToString();
    var currentRoles = await userManager.GetRolesAsync(user);

    var rolesToRemove = currentRoles
        .Where(r => managedRoles.Contains(r) && r != targetRole)
        .ToList();

    if (rolesToRemove.Count > 0)
    {
        await userManager.RemoveFromRolesAsync(user, rolesToRemove);
    }

    if (!currentRoles.Contains(targetRole))
    {
        await userManager.AddToRoleAsync(user, targetRole);
    }
}

public partial class Program { }

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public new const string Scheme = "TestAuth";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "john.smith"),
            new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString()),
            new Claim("FullName", "John Smith")
        };

        var identity = new ClaimsIdentity(claims, Scheme, ClaimTypes.Name, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
