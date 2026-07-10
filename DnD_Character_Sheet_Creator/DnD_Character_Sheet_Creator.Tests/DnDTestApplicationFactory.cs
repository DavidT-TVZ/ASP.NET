using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DnD_Character_Sheet_Creator.Tests
{
    public class DnDTestApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"DnDTestDb_{Guid.NewGuid():N}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Ensure the application picks up the Testing environment during builder creation
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            builder.ConfigureServices(services =>
            {
                // Remove the app's DbContext registrations (both options and context)
                var optionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DnDDbContext>));

                if (optionsDescriptor != null)
                {
                    services.Remove(optionsDescriptor);
                }

                var contextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DnDDbContext));

                if (contextDescriptor != null)
                {
                    services.Remove(contextDescriptor);
                }

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });

                // Add DbContext configured for in-memory database
                services.AddDbContext<DnDDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                    options.UseLazyLoadingProxies();
                });
            });

            builder.UseEnvironment("Testing");
        }

        public void SeedDatabase(DnDDbContext context)
        {
            // Reset the in-memory database to a clean state
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed test data
            var player = new Player
            {
                Name = "Test Player",
                Surname = "Player",
                Username = "testplayer",
                Email = "test@example.com",
                Password = "password",
                Role = RoleEnum.Admin
            };
            context.Players.Add(player);
            context.SaveChanges();

            var level = context.CharacterLevels.First(l => l.Level == 1);

            var character = new Character
            {
                PlayerId = player.PlayerId,
                CharacterName = "Test Character",
                Class = ClassEnum.Barbarian,
                Race = RaceEnum.Human,
                Background = BackgroundEnum.Acolyte,
                Alignment = AlignmentEnum.ChaoticEvil,
                LevelId = level.LevelId
            };
            context.Characters.Add(character);
            context.SaveChanges();

            var weapon = new Weapon
            {
                Name = "Test Sword",
                Type = "Melee",
                DamageAmount = "1",
                DamageType = "Slashing",
                WeaponProperties = new List<WeaponPropertiesEnum> { WeaponPropertiesEnum.Finesse },
                Weight = 2
            };
            weapon.CharacterId = character.CharacterId;
            character.EquipmentList.Add(weapon);
            context.Equipment.Add(weapon);
            context.SaveChanges();
        }
    }

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
                new Claim(ClaimTypes.Name, "testplayer"),
                new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString()),
                new Claim("FullName", "Test Player")
            };

            var identity = new ClaimsIdentity(claims, Scheme, ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
