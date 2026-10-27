using Microsoft.Extensions.Configuration;

namespace DummyJson.ApiTests.Config;

/// <summary>
/// Everything that might change between environments lives in appsettings.json,
/// so the steps never contain a URL or a password.
/// </summary>
public class TestSettings
{
    public string BaseUrl { get; set; } = "https://dummyjson.com/";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 30;

    // Loaded once and shared by every scenario.
    public static TestSettings Current { get; } = Load();

    private static TestSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return configuration.GetSection("DummyJson").Get<TestSettings>()
               ?? throw new InvalidOperationException("The DummyJson section is missing from appsettings.json");
    }
}
