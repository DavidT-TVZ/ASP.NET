using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DnD_Character_Sheet_Creator.Tests
{
    public class CharacterLevelsApiControllerTests : IClassFixture<DnDTestApplicationFactory>
    {
        private readonly DnDTestApplicationFactory _factory;
        private readonly HttpClient _client;

        public CharacterLevelsApiControllerTests(DnDTestApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithLevels()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
            }

            // Act
            var response = await _client.GetAsync("/api/character-levels");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var levels = await response.Content.ReadFromJsonAsync<IEnumerable<CharacterLevelDto>>();
            Assert.NotNull(levels);
            Assert.Single(levels);
            Assert.Equal(1, levels.First().Level);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);
                var level = context.CharacterLevels.First();
                var levelId = level.LevelId;

                // Act
                var response = await _client.GetAsync($"/api/character-levels/{levelId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<CharacterLevelDto>();
                Assert.NotNull(result);
                Assert.Equal(levelId, result.LevelId);
                Assert.Equal(1, result.Level);
            }
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/character-levels/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var newLevel = new CharacterLevelUpsertDto
            {
                Level = 2,
                ExperienceRequired = 300
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/character-levels", newLevel);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<CharacterLevelDto>();
            Assert.NotNull(result);
            Assert.Equal(2, result.Level);
            Assert.Equal(300, result.ExperienceRequired);
        }

        [Fact]
        public async Task Create_WithDuplicateLevel_ShouldReturnBadRequest()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);

                var duplicateLevel = new CharacterLevelUpsertDto
                {
                    Level = 1,
                    ExperienceRequired = 100
                };

                // Act
                var response = await _client.PostAsJsonAsync("/api/character-levels", duplicateLevel);

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
                var level = context.CharacterLevels.First();
                var levelId = level.LevelId;

                var updateDto = new CharacterLevelUpsertDto
                {
                    Level = 1,
                    ExperienceRequired = 50
                };

                // Act
                var response = await _client.PutAsJsonAsync($"/api/character-levels/{levelId}", updateDto);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadFromJsonAsync<CharacterLevelDto>();
                Assert.NotNull(result);
                Assert.Equal(50, result.ExperienceRequired);
            }
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var updateDto = new CharacterLevelUpsertDto
            {
                Level = 1,
                ExperienceRequired = 100
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/character-levels/99999", updateDto);

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
                var level = context.CharacterLevels.First();
                var levelId = level.LevelId;

                // Act
                var response = await _client.DeleteAsync($"/api/character-levels/{levelId}");

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                // Verify soft-deleted
                var deletedLevel = context.CharacterLevels.FirstOrDefault(l => l.LevelId == levelId);
                Assert.NotNull(deletedLevel);
                Assert.NotNull(deletedLevel.DeletedAt);
            }
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/character-levels/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Search_ShouldReturnFilteredLevels()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
                _factory.SeedDatabase(context);

                // Add another level
                var level2 = new CharacterLevel
                {
                    Level = 2,
                    ExperienceRequired = 300
                };
                context.CharacterLevels.Add(level2);
                context.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/character-levels?search=1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var levels = await response.Content.ReadFromJsonAsync<IEnumerable<CharacterLevelDto>>();
            Assert.NotNull(levels);
            Assert.Single(levels);
            Assert.Equal(1, levels.First().Level);
        }
    }
}
