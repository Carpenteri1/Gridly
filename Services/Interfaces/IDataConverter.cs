namespace Gridly.Services;

public interface IDataConverter<T>
{
    public T[]? DeserializeJsonToArray(string jsonString);
    public T? DeserializeJson(string jsonString);
    public string? SerializerToJsonString(T data);
    public string? SerializerToJsonString(IEnumerable<T> data);
    public int? ToInt(string data);
}        