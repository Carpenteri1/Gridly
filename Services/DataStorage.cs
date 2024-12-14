using System.Text.Json;
using Gridly.Models;

namespace Gridly.Services;

public class DataStorage
{
    private const string jsonComponentFileName = "componentData.json";
    private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", jsonComponentFileName);
    public static bool ReadToJsonFile(ComponentModel[] newComponent)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(newComponent);
            File.WriteAllText(filePath, jsonString);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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
        catch (NullReferenceException e) { Console.WriteLine(e.Message); }
        catch(JsonException e) { Console.WriteLine(e.Message); }
        catch (Exception e) { Console.WriteLine(e.Message); }
        
        return Array.Empty<ComponentModel>();
    }
}