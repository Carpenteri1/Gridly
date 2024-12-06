using System.Text.Json;
using Gridly.Models;

namespace Gridly.Handler;

public class LocalComponentHandler : ComponentModel
{
    public static IResult Save(ComponentModel newComponent)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(newComponent);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            File.WriteAllText(filePath, jsonString);

            return Results.Ok();
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }
}