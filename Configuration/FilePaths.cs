namespace Gridly.Configuration;

public static class FilePaths
{
    private const string JsonRateLimiterFileName = "rateLimiterData.json";
    private const string JsonComponentFileName = "componentData.json";

    public static readonly string RateLimitFilePath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/RateLimiterData", JsonRateLimiterFileName);
    public static readonly string ComponentPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", JsonComponentFileName);
    public static readonly string IconPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/Icons/");
}