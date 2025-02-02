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

    public static async Task<ComponentModel[]?> ReadAllFromJsonFile()
    {
        try
        {
            string jsonString = await File.ReadAllTextAsync(JsonPath);
            return DataConverter.DeserializeJsonString(jsonString);
        }
        catch (NullReferenceException e) { Console.WriteLine(e.Message); }
        catch(JsonException e) { Console.WriteLine(e.Message); }
        catch (Exception e) { Console.WriteLine(e.Message); }
        
        return ComponentModel.EmptyArray;
    }
    
    public static async Task<ComponentModel?> ReadByIdFromJsonFile(int Id)
    {
        try
        {
            string jsonString = await File.ReadAllTextAsync(JsonPath);
            return DataConverter.DeserializeJsonString(jsonString)?.ToList().First(x => x.Id == Id);
        }
        catch (NullReferenceException e) { Console.WriteLine(e.Message); }
        catch(JsonException e) { Console.WriteLine(e.Message); }
        catch (Exception e) { Console.WriteLine(e.Message); }
        
        return null;
    }

    public static bool WriteIconToFolder(IconModel iconData)
    {
        string filePath = IconPath + $"{iconData.name}.{iconData.fileType}";
        if (File.Exists(filePath)) return true;
        
        try
        {
            File.WriteAllBytes(filePath, Convert.FromBase64String(iconData.base64Data));
        }
        catch (Exception e) { Console.WriteLine(e.Message); return false; }

        return true;
    }
    
    public static bool DeleteIconFromFolder(IconModel iconData)
    {
        string filePath = IconPath + $"{iconData.name}.{iconData.fileType}";
        
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

    public static bool IconExcistOnOtherComponent(List<ComponentModel>componentModels, IconModel iconData) =>
        !componentModels.Where(x => x.IconData != null &&
                                    x.IconData.name == iconData.name &&
                                    x.IconData.fileType == iconData.fileType).Any();
    
}