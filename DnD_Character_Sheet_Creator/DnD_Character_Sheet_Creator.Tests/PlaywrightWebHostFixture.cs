using System;
using System.Diagnostics;
using System.IO;

namespace DnD_Character_Sheet_Creator.Tests
{
    public sealed class PlaywrightWebHostFixture : IAsyncLifetime
    {
        private Process? _process;

        public async Task InitializeAsync()
        {
            var webProjectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DnD_Character_Sheet_Creator.Web", "DnD_Character_Sheet_Creator.Web.csproj"));
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{webProjectPath}\" --no-build --no-launch-profile --urls http://127.0.0.1:5055",
                WorkingDirectory = Path.GetDirectoryName(webProjectPath) ?? AppContext.BaseDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";
            startInfo.Environment["PLAYWRIGHT_BASE_URL"] = PlaywrightTestConfig.BaseUrl;

            _process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start the web host process.");

            var started = false;
            var deadline = DateTime.UtcNow.AddSeconds(60);

            while (DateTime.UtcNow < deadline)
            {
                if (_process.HasExited)
                {
                    throw new InvalidOperationException($"Web host exited early with code {_process.ExitCode}.");
                }

                try
                {
                    using var client = new HttpClient();
                    var response = await client.GetAsync(PlaywrightTestConfig.BaseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        started = true;
                        break;
                    }
                }
                catch
                {
                }

                await Task.Delay(1000);
            }

            if (!started)
            {
                throw new InvalidOperationException("The Playwright web host did not start in time.");
            }
        }

        public Task DisposeAsync()
        {
            if (_process != null && !_process.HasExited)
            {
                try
                {
                    _process.Kill(true);
                }
                catch
                {
                }
            }

            _process?.Dispose();
            return Task.CompletedTask;
        }
    }
}