using System.Text.Json;
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
}