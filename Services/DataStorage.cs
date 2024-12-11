using System.Text.Json;
using Gridly.Models;

namespace Gridly.Services;

public class DataStorage
{
    private const string fileName = "componentData.json";
    private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

    public static bool ReadToJsonFile(ComponentModel[] newComponent)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(newComponent);
            File.WriteAllText(filePath, jsonString);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static async Task<ComponentModel[]> ReadFromJsonFile()
    {
        try
        {
            string jsonString = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<ComponentModel[]>(jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Array.Empty<ComponentModel>();
        }
    }
}