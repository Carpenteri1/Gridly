namespace Gridly.Services;

public interface IDataConverter<T>
{
    public T[]? DeserializeJsonStringArray(string jsonString);
    public T? DeserializeJsonString(string jsonString);
    public string? SerializerToJsonString(T data);
    public string? SerializerToJsonString(List<T> data);
}        