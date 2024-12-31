using System.Text.Json;
using Gridly.Models;

namespace Gridly.Services;

public class DataStorage
{
    private const string jsonComponentFileName = "componentData.json";
    private static readonly string JsonPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", jsonComponentFileName);
    private static readonly string IconPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/Icons");
    
    public static bool ReadToJsonFile(List<ComponentModel> newComponent)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(newComponent);
            File.WriteAllText(JsonPath, jsonString);    

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
            string jsonString = await File.ReadAllTextAsync(JsonPath);
            return JsonSerializer.Deserialize<ComponentModel[]>(jsonString);
        }
        catch (NullReferenceException e) { Console.WriteLine(e.Message); }
        catch(JsonException e) { Console.WriteLine(e.Message); }
        catch (Exception e) { Console.WriteLine(e.Message); }
        
        return ComponentModel.EmptyArray;
    }

    public static bool DecryptBase64String(IconModel iconData)
    {
        try
        {
            var imageBytes = Convert.FromBase64String(iconData.Base64Data);
            string filePath = IconPath +$"{iconData.Name}.{iconData.FileType}";                    
            File.WriteAllBytes(filePath, imageBytes);   
            
        }catch(Exception e) { Console.WriteLine(e.Message); return false; }

        return true;
    }
}