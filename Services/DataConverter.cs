using System.Text.Json;
using System.Text.RegularExpressions;

namespace Gridly.Services;

public class DataConverter<T> : IDataConverter<T>
{
    public T[]? DeserializeJsonToArray(string jsonString) => 
        JsonSerializer.Deserialize<T[]>(jsonString);
    public T? DeserializeJson(string jsonString) => 
        JsonSerializer.Deserialize<T>(jsonString);
    public string? SerializerToJsonString(T data) => 
        JsonSerializer.Serialize(data);
    public string? SerializerToJsonString(IEnumerable<T> data) => 
        JsonSerializer.Serialize(data);
    public int? ToInt(string data) => 
        int.Parse(Regex.Replace(data, @"[^0-9]", ""));
}