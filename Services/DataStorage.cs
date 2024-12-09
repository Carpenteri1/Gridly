using System.Text.Json;

namespace Gridly.Services;

public class DataStorage
{
    private const string fileName = "componentData.json";

    public static bool ReadToJsonFile(dynamic newComponent)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(newComponent);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(filePath, jsonString);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static async Task<dynamic[]> ReadFromJsonFile()
    {
        try
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            string jsonString = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<dynamic[]>(jsonString);
        }
        catch (Exception)
        {
            return Array.Empty<dynamic>();
        }
    }
}