using System.Text.Json.Serialization;

namespace Gridly.Models;

public class ComponentModel
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("index")] public int Index { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("iconData")] public IconModel? IconData { get; set; }
    [JsonPropertyName("iconUrl")] public string IconUrl { get; set; }
    [JsonPropertyName("componentSettings")] public ComponentSettingsModel? ComponentSettings { get; set; }
}