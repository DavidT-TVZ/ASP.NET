using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

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
                Password = "password"
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
}
