using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using System;
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DnD_Character_Sheet_Creator.Tests
{
    public class EquipmentApiControllerTests : IClassFixture<DnDTestApplicationFactory>
    {
        private readonly DnDTestApplicationFactory _factory;
        private readonly HttpClient _client;

        public EquipmentApiControllerTests(DnDTestApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithEquipment()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
            }

            // Act
            var response = await _client.GetAsync("/api/equipment");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var equipment = await response.Content.ReadFromJsonAsync<IEnumerable<EquipmentDto>>();
            Assert.NotNull(equipment);
            Assert.Contains(equipment, e => e.Name == "Test Sword");
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var equipment = context.Equipment.First(e => e.Name == "Test Sword");
                var equipmentId = equipment.EquipmentId;

                // Act
                var response = await _client.GetAsync($"/api/equipment/{equipmentId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<EquipmentDto>();
                Assert.NotNull(result);
                Assert.Equal(equipmentId, result.EquipmentId);
                Assert.Equal("Test Sword", result.Name);
            }
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/equipment/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var character = context.Characters.First();

                var newEquipment = new EquipmentUpsertDto
                {
                    CharacterId = character.CharacterId,
                    Name = "New Armor",
                    Type = "Armor",
                    Cost = 100,
                    Weight = 50
                };

                // Act
                var response = await _client.PostAsJsonAsync("/api/equipment", newEquipment);

                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<EquipmentDto>();
                Assert.NotNull(result);
                Assert.Equal("New Armor", result.Name);
                Assert.Equal("Armor", result.Type);

                return;
            }
        }

        [Fact]
        public async Task Update_WithValidData_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var equipment = context.Equipment.First(e => e.Name == "Test Sword");
                var equipmentId = equipment.EquipmentId;
                var linkedCharacterId = equipment.CharacterId ?? equipment.Characters.First().CharacterId;

                var updateDto = new EquipmentUpsertDto
                {
                    CharacterId = linkedCharacterId,
                    Name = "Updated Sword",
                    Type = "Melee Weapon",
                    Cost = 0,
                    Weight = 3
                };

                // Act
                var response = await _client.PutAsJsonAsync($"/api/equipment/{equipmentId}", updateDto);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<EquipmentDto>();
                Assert.NotNull(result);
                Assert.Equal("Updated Sword", result.Name);
            }
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var updateDto = new EquipmentUpsertDto
            {
                Name = "Updated Equipment",
                CharacterId = 1,
                Type = "Test",
                Cost = 0,
                Weight = 5
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/equipment/99999", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var equipment = context.Equipment.First(e => e.Name == "Test Sword");
                var equipmentId = equipment.EquipmentId;

                // Act
                var response = await _client.DeleteAsync($"/api/equipment/{equipmentId}");

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                // Verify soft-deleted
                using var verifyScope = _factory.Services.CreateScope();
                var verifyContext = verifyScope.ServiceProvider.GetRequiredService<DnDDbContext>();
                var deletedEquipment = verifyContext.Equipment.AsNoTracking().FirstOrDefault(e => e.EquipmentId == equipmentId);
                Assert.NotNull(deletedEquipment);
                Assert.NotNull(deletedEquipment.DeletedAt);
            }
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/equipment/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Search_ShouldReturnFilteredEquipment()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);

                // Add another equipment
                var shield = new Armour
                {
                    Name = "Test Shield",
                    Type = "Armor",
                    ArmourClass = 1,
                    Cost = 0,
                    Weight = 6
                };
                context.Equipment.Add(shield);
                context.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/equipment?search=Test Sword");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var equipment = await response.Content.ReadFromJsonAsync<IEnumerable<EquipmentDto>>();
            Assert.NotNull(equipment);
            Assert.Single(equipment);
            Assert.Equal("Test Sword", equipment.First().Name);
        }
    }
}
