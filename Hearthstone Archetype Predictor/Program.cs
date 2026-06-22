using System;
using Avalonia;
using Microsoft.Extensions.Configuration;

namespace Hearthstone_Archetype_Predictor;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        LoadConfiguration();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Loads configuration from user-secrets (local) or enviroment variables (CI/production).
    ///
    /// Local:
    ///     dotnet user-secrets init
    ///     dotnet user-secrets set "AzureML:ApiKey" "your_api_key"
    ///
    /// CI (Github Actions):
    ///     env:
    ///         AZURE_ML_API_KEY: ${{ secrets.AZURE_ML_API_KEY }}
    /// </summary>
    private static void LoadConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        AppConfig.AzureMLApiKey = config["AzureML:ApiKey"] ?? string.Empty;

        if (string.IsNullOrEmpty(AppConfig.AzureMLApiKey))
            Console.WriteLine(
                "[WARNING] AzureML:ApiKey isn't set. Archetype prediction will not work."
            );
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
