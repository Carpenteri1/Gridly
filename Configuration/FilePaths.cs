namespace Gridly.Configuration;

public static class FilePaths
{
    private const string VersionDataFileName = "versionData.json";
    private const string JsonComponentFileName = "componentData.json";

    public static readonly string VersionFilePath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/VersionData", VersionDataFileName);
    public static readonly string ComponentPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", JsonComponentFileName);
    public static readonly string IconPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/Icons/");
}