using System;

namespace DnD_Character_Sheet_Creator.Tests
{
    internal static class PlaywrightTestConfig
    {
        public static string BaseUrl => Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://127.0.0.1:5055";
    }
}