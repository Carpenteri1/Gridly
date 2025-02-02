using System.Text.Json;
using Gridly.Models;

namespace Gridly.Services;

public class DataConverter
{
    public static ComponentModel[]? DeserializeJsonString(string jsonString) => 
        JsonSerializer.Deserialize<ComponentModel[]>(jsonString);
}