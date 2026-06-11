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
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DnDDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add DbContext configured for in-memory database
                services.AddDbContext<DnDDbContext>(options =>
                {
                    options.UseInMemoryDatabase("DnDTestDb");
                    options.UseLazyLoadingProxies();
                });
            });

            builder.UseEnvironment("Development");
        }

        public void SeedDatabase(DnDDbContext context)
        {
            // Clear existing data
            context.Players.RemoveRange(context.Players);
            context.Characters.RemoveRange(context.Characters);
            context.Equipment.RemoveRange(context.Equipment);
            context.CharacterLevels.RemoveRange(context.CharacterLevels);
            context.SaveChanges();

            // Seed test data
            var player = new Player
            {
                Name = "Test Player",
                Email = "test@example.com",
                OIB = "12345678901"
            };
            context.Players.Add(player);
            context.SaveChanges();

            var level = new CharacterLevel
            {
                Level = 1,
                ExperienceRequired = 0
            };
            context.CharacterLevels.Add(level);
            context.SaveChanges();

            var character = new Character
            {
                PlayerId = player.PlayerId,
                Name = "Test Character",
                Class = ClassEnum.Barbarian,
                Race = RaceEnum.Human,
                Background = BackgroundEnum.Acolyte,
                Alignment = AlignmentEnum.ChaoticEvil,
                LevelId = level.LevelId,
                Health = 10,
                ExperiencePoints = 0
            };
            context.Characters.Add(character);
            context.SaveChanges();

            var weapon = new Weapon
            {
                Name = "Test Sword",
                Type = "Melee",
                DamageAmount = 1,
                DamageType = "Slashing",
                WeaponProperties = WeaponPropertiesEnum.Finesse,
                Weight = 2.0
            };
            context.Equipment.Add(weapon);
            context.SaveChanges();
        }
    }
}
