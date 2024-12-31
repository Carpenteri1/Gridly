using System.Text.Json.Serialization;

namespace Gridly.Models;

public class ComponentModel
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("iconData")] public IconModel? IconData { get; set; }
    [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }
    public static ComponentModel[] EmptyArray => [];
}
public  record IconModel(string name, string fileType, string base64Data);