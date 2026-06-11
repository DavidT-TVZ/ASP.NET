using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using System;
using System.Net;
using System.Net.Http.Json;
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
            Assert.Single(equipment);
            Assert.Equal("Test Sword", equipment.First().Name);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var equipment = context.Equipment.First();
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
            var newEquipment = new EquipmentUpsertDto
            {
                Name = "New Armor",
                Type = "Armor",
                ArmorClass = 14,
                Weight = 50.0
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/equipment", newEquipment);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<EquipmentDto>();
            Assert.NotNull(result);
            Assert.Equal("New Armor", result.Name);
            Assert.Equal("Armor", result.Type);
        }

        [Fact]
        public async Task Update_WithValidData_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var equipment = context.Equipment.First();
                var equipmentId = equipment.EquipmentId;

                var updateDto = new EquipmentUpsertDto
                {
                    Name = "Updated Sword",
                    Type = "Melee Weapon",
                    Weight = 3.0
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
                Type = "Test",
                Weight = 5.0
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
                var equipment = context.Equipment.First();
                var equipmentId = equipment.EquipmentId;

                // Act
                var response = await _client.DeleteAsync($"/api/equipment/{equipmentId}");

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                // Verify soft-deleted
                var deletedEquipment = context.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId);
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
                    ArmorClass = 1,
                    Weight = 6.0
                };
                context.Equipment.Add(shield);
                context.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/equipment?search=Sword");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var equipment = await response.Content.ReadFromJsonAsync<IEnumerable<EquipmentDto>>();
            Assert.NotNull(equipment);
            Assert.Single(equipment);
            Assert.Equal("Test Sword", equipment.First().Name);
        }
    }
}
