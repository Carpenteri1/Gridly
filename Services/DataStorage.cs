using System.Text.Json;
using Gridly.Models;

namespace Gridly.Services;

public class DataStorage
{
    private const string JsonComponentFileName = "componentData.json";
    private static readonly string JsonPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", JsonComponentFileName);
    private static readonly string IconPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/Icons/");
    
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

    public static bool WriteIconToFolder(IconModel iconData)
    {
        string filePath = IconPath + $"{iconData.Name}.{iconData.FileType}";
        if (File.Exists(filePath)) return true;
        
        try
        {
            File.WriteAllBytes(filePath, Convert.FromBase64String(iconData.Base64Data));
        }
        catch (Exception e) { Console.WriteLine(e.Message); return false; }

        return true;
    }
    
    public static bool DeleteIconFromFolder(IconModel iconData)
    {
        string filePath = IconPath + $"{iconData.Name}.{iconData.FileType}";
        
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }   
        }

        return true;
    }
}