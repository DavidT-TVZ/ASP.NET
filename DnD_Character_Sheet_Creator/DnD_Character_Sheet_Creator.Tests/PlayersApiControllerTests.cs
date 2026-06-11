using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DnD_Character_Sheet_Creator.Tests
{
    public class PlayersApiControllerTests : IClassFixture<DnDTestApplicationFactory>
    {
        private readonly DnDTestApplicationFactory _factory;
        private readonly HttpClient _client;

        public PlayersApiControllerTests(DnDTestApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithPlayers()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
            }

            // Act
            var response = await _client.GetAsync("/api/players");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var players = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerDto>>();
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var player = context.Players.First();
                var playerId = player.PlayerId;

                // Act
                var response = await _client.GetAsync($"/api/players/{playerId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<PlayerDto>();
                Assert.NotNull(result);
                Assert.Equal(playerId, result.PlayerId);
                Assert.Equal("Test Player", result.Name);
            }
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/players/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var newPlayer = new PlayerUpsertDto
            {
                Name = "New Player",
                Surname = "Player",
                Username = "newplayer",
                Email = "newplayer@example.com",
                Password = "password"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/players", newPlayer);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<PlayerDto>();
            Assert.NotNull(result);
            Assert.Equal("New Player", result.Name);
            Assert.Equal("newplayer@example.com", result.Email);

            // Verify in database
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                var dbPlayer = context.Players.FirstOrDefault(p => p.Name == "New Player");
                Assert.NotNull(dbPlayer);
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
                var player = context.Players.First();
                var playerId = player.PlayerId;

                var updateDto = new PlayerUpsertDto
                {
                    Name = "Updated Player",
                    Surname = "Player",
                    Username = "updatedplayer",
                    Email = "updated@example.com",
                    Password = "newpass"
                };

                // Act
                var response = await _client.PutAsJsonAsync($"/api/players/{playerId}", updateDto);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<PlayerDto>();
                Assert.NotNull(result);
                Assert.Equal("Updated Player", result.Name);
                Assert.Equal("updated@example.com", result.Email);
            }
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var updateDto = new PlayerUpsertDto
            {
                Name = "Updated Player",
                Surname = "Player",
                Username = "updatedplayer",
                Email = "updated@example.com",
                Password = "newpass"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/players/99999", updateDto);

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
                var player = context.Players.First();
                var playerId = player.PlayerId;

                // Act
                var response = await _client.DeleteAsync($"/api/players/{playerId}");

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                // Verify soft-deleted in database
                var deletedPlayer = context.Players.FirstOrDefault(p => p.PlayerId == playerId);
                Assert.NotNull(deletedPlayer);
                Assert.NotNull(deletedPlayer.DeletedAt);
            }
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/players/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Search_ShouldReturnFilteredPlayers()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);

                // Add another player
                var player2 = new Player
                {
                    Name = "Another Player",
                    Surname = "Player",
                    Username = "another",
                    Email = "another@example.com",
                    Password = "password"
                };
                context.Players.Add(player2);
                context.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/players?search=Test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var players = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerDto>>();
            Assert.NotNull(players);
            Assert.Single(players);
            Assert.Equal("Test Player", players.First().Name);
        }
    }
}
