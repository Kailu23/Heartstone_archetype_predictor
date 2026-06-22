namespace Hearthstone_Archetype_Predictor;

/// <summary>
/// Global app configuration.
/// Entered in Program.cs from user-secrets or enviroment variables.
/// </summary>
public class AppConfig
{
    public static string AzureMLApiKey { get; set; } = string.Empty;
}
