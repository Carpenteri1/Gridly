using System.Text.Json.Serialization;

namespace Gridly.Models;

public class ComponentModel
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("iconData")] public IconModel? IconData { get; set; }
    [JsonPropertyName("imageUrl")] public string ImageUrl { get; set; }
    [JsonPropertyName("titleHidden")] public bool TitleHidden { get; set; }
    [JsonPropertyName("imageHidden")] public bool ImageHidden { get; set; }
    [JsonPropertyName("componentSettings")] public ComponentSettingsModel? ComponentSettings { get; set; }
}
public record IconModel(string name, string type, string base64Data);
public record ComponentSettingsModel(int width, int height);