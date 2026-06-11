using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DnD_Character_Sheet_Creator.Tests
{
    public class CharactersApiControllerTests : IClassFixture<DnDTestApplicationFactory>
    {
        private readonly DnDTestApplicationFactory _factory;
        private readonly HttpClient _client;

        public CharactersApiControllerTests(DnDTestApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithCharacters()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
            }

            // Act
            var response = await _client.GetAsync("/api/characters");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var characters = await response.Content.ReadFromJsonAsync<IEnumerable<CharacterDto>>();
            Assert.NotNull(characters);
            Assert.Single(characters);
            Assert.Equal("Test Character", characters.First().CharacterName);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var character = context.Characters.First();
                var characterId = character.CharacterId;

                // Act
                var response = await _client.GetAsync($"/api/characters/{characterId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<CharacterDto>();
                Assert.NotNull(result);
                Assert.Equal(characterId, result.CharacterId);
                Assert.Equal("Test Character", result.CharacterName);
            }
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/characters/99999");

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
                var player = context.Players.First();
                var level = context.CharacterLevels.First();

                var newCharacter = new CharacterUpsertDto
                {
                    CharacterName = "New Character",
                    PlayerId = player.PlayerId,
                    Class = ClassEnum.Barbarian,
                    Race = RaceEnum.Human,
                    Background = BackgroundEnum.Acolyte,
                    Alignment = AlignmentEnum.ChaoticEvil,
                    LevelId = level.LevelId
                };

                // Act
                var response = await _client.PostAsJsonAsync("/api/characters", newCharacter);

                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<CharacterDto>();
                Assert.NotNull(result);
                Assert.Equal("New Character", result.CharacterName);
            }
        }

        [Fact]
        public async Task Create_WithInvalidPlayerId_ShouldReturnBadRequest()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var level = context.CharacterLevels.First();

                var newCharacter = new CharacterUpsertDto
                {
                    CharacterName = "New Character",
                    PlayerId = 99999,
                    Class = ClassEnum.Barbarian,
                    Race = RaceEnum.Human,
                    Background = BackgroundEnum.Acolyte,
                    Alignment = AlignmentEnum.ChaoticEvil,
                    LevelId = level.LevelId
                };

                // Act
                var response = await _client.PostAsJsonAsync("/api/characters", newCharacter);

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
                var character = context.Characters.First();
                var characterId = character.CharacterId;
                var player = context.Players.First();
                var level = context.CharacterLevels.First();

                var updateDto = new CharacterUpsertDto
                {
                    CharacterName = "Updated Character",
                    PlayerId = player.PlayerId,
                    Class = ClassEnum.Bard,
                    Race = RaceEnum.Elf,
                    Background = BackgroundEnum.Acolyte,
                    Alignment = AlignmentEnum.ChaoticEvil,
                    LevelId = level.LevelId
                };

                // Act
                var response = await _client.PutAsJsonAsync($"/api/characters/{characterId}", updateDto);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<CharacterDto>();
                Assert.NotNull(result);
                Assert.Equal("Updated Character", result.CharacterName);
                Assert.Equal(ClassEnum.Bard, result.Class);
            }
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var player = context.Players.First();
                var level = context.CharacterLevels.First();

                var updateDto = new CharacterUpsertDto
                {
                    CharacterName = "Updated Character",
                    PlayerId = player.PlayerId,
                    Class = ClassEnum.Barbarian,
                    Race = RaceEnum.Human,
                    Background = BackgroundEnum.Acolyte,
                    Alignment = AlignmentEnum.ChaoticEvil,
                    LevelId = level.LevelId
                };

                // Act
                var response = await _client.PutAsJsonAsync("/api/characters/99999", updateDto);

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var character = context.Characters.First();
                var characterId = character.CharacterId;

                // Act
                var response = await _client.DeleteAsync($"/api/characters/{characterId}");

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                // Verify soft-deleted
                var deletedCharacter = context.Characters.FirstOrDefault(c => c.CharacterId == characterId);
                Assert.NotNull(deletedCharacter);
                Assert.NotNull(deletedCharacter.DeletedAt);
            }
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/characters/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Search_ShouldReturnFilteredCharacters()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);

                // Add another character
                var player = context.Players.First();
                var level = context.CharacterLevels.First();
                var character2 = new Character
                {
                    PlayerId = player.PlayerId,
                    CharacterName = "Another Character",
                    Class = ClassEnum.Cleric,
                    Race = RaceEnum.Dwarf,
                    Background = BackgroundEnum.Soldier,
                    Alignment = AlignmentEnum.NeutralGood,
                    LevelId = level.LevelId
                };
                context.Characters.Add(character2);
                context.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/characters?search=Test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var characters = await response.Content.ReadFromJsonAsync<IEnumerable<CharacterDto>>();
            Assert.NotNull(characters);
            Assert.Single(characters);
            Assert.Equal("Test Character", characters.First().CharacterName);
        }
    }
}
