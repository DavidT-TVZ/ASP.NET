using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace DnD_Character_Sheet_Creator.Tests
{
    [Collection("Playwright")]
    public class PlaywrightResponsiveLayoutTests : IClassFixture<PlaywrightWebHostFixture>, IAsyncLifetime
    {
        private readonly PlaywrightWebHostFixture _webHostFixture;
        private IPlaywright? _playwright;
        private IBrowser? _browser;

        public PlaywrightResponsiveLayoutTests(PlaywrightWebHostFixture webHostFixture)
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
        public async Task MobileViewport_ShouldNotIntroduceHorizontalOverflow_OnKeyPages()
        {
            if (_browser == null)
            {
                throw new InvalidOperationException("Playwright browser was not initialized.");
            }

            var page = await _browser.NewPageAsync(new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize { Width = 375, Height = 812 }
            });

            string[] urls =
            {
                $"{PlaywrightTestConfig.BaseUrl}/",
                $"{PlaywrightTestConfig.BaseUrl}/Codex",
                $"{PlaywrightTestConfig.BaseUrl}/Players",
                $"{PlaywrightTestConfig.BaseUrl}/Characters",
                $"{PlaywrightTestConfig.BaseUrl}/Players/Details/1",
                $"{PlaywrightTestConfig.BaseUrl}/Characters/Details/1"
            };

            foreach (var url in urls)
            {
                await page.GotoAsync(url);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var metrics = await page.EvaluateAsync<OverflowMetrics>(@"() => ({
                    viewportWidth: window.innerWidth,
                    documentWidth: document.documentElement.scrollWidth,
                    bodyWidth: document.body ? document.body.scrollWidth : 0
                })");

                var widest = Math.Max(metrics.DocumentWidth, metrics.BodyWidth);

                // Allow a tiny tolerance for browser rounding to avoid flaky failures.
                Assert.True(
                    widest <= metrics.ViewportWidth + 2,
                    $"Horizontal overflow detected on '{url}'. Viewport: {metrics.ViewportWidth}, content: {widest}.");
            }
        }

        [Fact]
        public async Task MobileViewport_PrimaryCreateButtons_ShouldBeVisibleAndEnabled()
        {
            if (_browser == null)
            {
                throw new InvalidOperationException("Playwright browser was not initialized.");
            }

            var page = await _browser.NewPageAsync(new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize { Width = 375, Height = 812 }
            });

            string[] formUrls =
            {
                $"{PlaywrightTestConfig.BaseUrl}/Players/Create",
                $"{PlaywrightTestConfig.BaseUrl}/Characters/Create?playerId=1"
            };

            foreach (var url in formUrls)
            {
                await page.GotoAsync(url);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var primaryCreateButton = page.Locator(".btn-create").First;
                await primaryCreateButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

                Assert.True(await primaryCreateButton.IsVisibleAsync(), $"Primary create button is not visible on '{url}'.");
                Assert.True(await primaryCreateButton.IsEnabledAsync(), $"Primary create button is not enabled on '{url}'.");
            }
        }

        private sealed class OverflowMetrics
        {
            public int ViewportWidth { get; set; }
            public int DocumentWidth { get; set; }
            public int BodyWidth { get; set; }
        }
    }
}
