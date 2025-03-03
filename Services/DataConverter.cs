using System.Text.Json;
using System.Text.RegularExpressions;

namespace Gridly.Services;

public class DataConverter<T> : IDataConverter<T>
{
    public T[]? DeserializeJsonStringArray(string jsonString) => 
        JsonSerializer.Deserialize<T[]>(jsonString);
    public T? DeserializeJsonString(string jsonString) => 
        JsonSerializer.Deserialize<T>(jsonString);
    public string? SerializerToJsonString(T data) => 
        JsonSerializer.Serialize(data);
    public string? SerializerToJsonString(List<T> data) => 
        JsonSerializer.Serialize(data);
    public int? ToInt(string data) => 
        int.Parse(Regex.Replace(data, @"[^0-9]", ""));
}