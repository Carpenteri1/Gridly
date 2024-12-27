using System.Text.Json.Serialization;

namespace Gridly.Models;

public class ComponentModel
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("iconFile")] public IFormFile? IconeFile { get; set; }
    [JsonPropertyName("iconUrl")] public string? IconUrl { get; set; }
}