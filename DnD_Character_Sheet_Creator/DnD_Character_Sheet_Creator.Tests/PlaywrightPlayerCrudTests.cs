using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnD_Character_Sheet_Creator.Tests
{
    [Collection("Playwright")]
    public class PlaywrightPlayerCrudTests : IClassFixture<PlaywrightWebHostFixture>, IAsyncLifetime
    {
        private readonly PlaywrightWebHostFixture _webHostFixture;
        private IPlaywright? _playwright;
        private IBrowser? _browser;

        public PlaywrightPlayerCrudTests(PlaywrightWebHostFixture webHostFixture)
        {
            _webHostFixture = webHostFixture;
        }

        public async Task InitializeAsync()
        {
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
        }

        public async Task DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }

            _playwright?.Dispose();
        }

        [Fact]
        public async Task PlayerCrudScenario_ShouldCreateEditAndRemovePlayer()
        {
            if (_browser == null)
            {
                throw new InvalidOperationException("Playwright browser was not initialized.");
            }

            var page = await _browser.NewPageAsync(new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize { Width = 1440, Height = 1200 }
            });

            var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
            var firstName = $"Playwright{uniqueSuffix}";
            var lastName = "Tester";
            var username = $"playwright{uniqueSuffix}";
            var email = $"{username}@example.com";

            await page.GotoAsync($"{PlaywrightTestConfig.BaseUrl}/Players/Create");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var createPageUrl = page.Url;
            var createPageText = await page.Locator("body").InnerTextAsync();
            Assert.Contains("Guild Registration", createPageText);
            await page.FillAsync("input[name='Name']", firstName);
            await page.FillAsync("input[name='Surname']", lastName);
            await page.FillAsync("input[name='Username']", username);
            await page.FillAsync("input[name='Email']", email);
            await page.GetByRole(AriaRole.Button, new() { Name = "Create Player" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().Contains("/Players/Details/", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().Contains("/Actors/Details/", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            var createdPlayerId = int.Parse(page.Url.Split('/').Last());

            await page.ClickAsync("a[href*='/Players/Edit/'], a[href*='/Actors/Edit/']");
            await page.FillAsync("input[name='Surname']", "Edited");
            await page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().Contains("/Players/Details/", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().Contains("/Actors/Details/", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            var detailsText = await page.Locator("body").InnerTextAsync();
            Assert.Contains($"{firstName} Edited", detailsText);

            await page.ClickAsync("a[href*='/Players/Remove/'], a[href*='/Actors/Remove/']");
            await page.GetByRole(AriaRole.Button, new() { Name = "Remove Player" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().EndsWith("/Players", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().EndsWith("/Actors", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            var listText = await page.Locator("body").InnerTextAsync();
            Assert.DoesNotContain($"{firstName} {lastName}", listText, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(username, listText, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CharacterCrudScenario_ShouldCreateAddEquipmentEditAndRemoveCharacter()
        {
            if (_browser == null)
            {
                throw new InvalidOperationException("Playwright browser was not initialized.");
            }

            var page = await _browser.NewPageAsync(new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize { Width = 1440, Height = 1200 }
            });

            var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
            var characterName = $"Playwright Hero {uniqueSuffix}";

            await page.GotoAsync($"{PlaywrightTestConfig.BaseUrl}/Characters/Create?playerId=1");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var characterCreatePageUrl = page.Url;
            var characterCreatePageText = await page.Locator("body").InnerTextAsync();
            Assert.Contains("New Hero", characterCreatePageText);
            await page.FillAsync("input[name='CharacterName']", characterName);
            await page.SelectOptionAsync("select[name='Race']", new[] { "Human" });
            await page.SelectOptionAsync("select[name='Background']", new[] { "Acolyte" });
            await page.SelectOptionAsync("select[name='Alignment']", new[] { "ChaoticEvil" });
            await page.SelectOptionAsync("select[name='Class']", new[] { "Barbarian" });
            await page.FillAsync("input[name='CurrentExperiencePoints']", "0");
            await page.FillAsync("input[name='DateOfLastLevelUp']", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm"));
            await page.GetByRole(AriaRole.Button, new() { Name = "Create Character" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().Contains("/Characters/Details/", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().Contains("/Adventurers/Details/", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            var createdCharacterId = int.Parse(page.Url.Split('/').Last());

            await page.ClickAsync("a[href*='/equipment/create-form']");
            await page.FillAsync("input[name='Name']", "Test Dagger");
            await page.FillAsync("input[name='Type']", "Weapon");
            await page.FillAsync("input[name='Cost']", "2");
            await page.FillAsync("input[name='Weight']", "1");
            await page.GetByRole(AriaRole.Button, new() { Name = "Add equipment" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().Contains($"/Characters/Details/{createdCharacterId}", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().Contains($"/Adventurers/Details/{createdCharacterId}", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            await Expect(page.GetByText("Test Dagger")).ToBeVisibleAsync();

            await page.ClickAsync("a[href*='/Characters/Edit/'], a[href*='/Adventurers/Edit/']");
            await page.FillAsync("input[name='CharacterName']", $"{characterName} Edited");
            await page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().Contains($"/Characters/Details/{createdCharacterId}", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().Contains($"/Adventurers/Details/{createdCharacterId}", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            var detailsText = await page.Locator("body").InnerTextAsync();
            Assert.Contains("Edited", detailsText);

            await page.ClickAsync("a[href*='/Characters/Remove/'], a[href*='/Adventurers/Remove/']");
            await page.GetByRole(AriaRole.Button, new() { Name = "Remove Character" }).ClickAsync();

            await page.WaitForURLAsync(url =>
                url.ToString().EndsWith("/Characters", StringComparison.OrdinalIgnoreCase) ||
                url.ToString().EndsWith("/Adventurers", StringComparison.OrdinalIgnoreCase), new PageWaitForURLOptions
            {
                Timeout = 15000
            });

            var listText = await page.Locator("body").InnerTextAsync();
            Assert.DoesNotContain(createdCharacterId.ToString(), listText);
            Assert.DoesNotContain(characterName, listText);
        }

    }
}