using System.Text.Json;

namespace Gridly.Handler;

public class LocalComponentHandler
{
    private const string fileName = "componentData.json";
    public static IResult Save(dynamic newComponent)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(newComponent);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(filePath, jsonString);

            return Results.Ok();
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }
    
    public static async Task<dynamic[]> Get()
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