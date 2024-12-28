using System.Text.Json;
using Gridly.Models;

namespace Gridly.Services;

public class DataStorage
{
    private const string jsonComponentFileName = "componentData.json";
    private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", jsonComponentFileName);
    public static bool ReadToJsonFile(List<ComponentModel> newComponent)
    {
        /*foreach (var component in newComponent)
        {
            
            var fileData = Convert.FromBase64String(component.IconData.Split(',')[1]); // Decode Base64
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var filePath = Path.Combine(uploadsFolderPath, $"{component.Name}.png"); // Save as a PNG
            System.IO.File.WriteAllBytes(filePath, fileData);
        }*/
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

    public static async Task<ComponentModel[]?> ReadFromJsonFile()
    {
        try
        {
            string jsonString = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<ComponentModel[]>(jsonString);
        }
        catch (NullReferenceException e) { Console.WriteLine(e.Message); }
        catch(JsonException e) { Console.WriteLine(e.Message); }
        catch (Exception e) { Console.WriteLine(e.Message); }
        
        return null;
    }
}