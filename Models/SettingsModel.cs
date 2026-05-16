using System.Text.Json.Serialization;

namespace Gridly.Models;

public class SettingsModel
{
    [JsonPropertyName("id")] public int? Id { get; set; } 
    [JsonPropertyName("cardId")] public int? CardId { get; set; }
    [JsonPropertyName("width")] public int Width { get; set; }
    [JsonPropertyName("height")] public int Height { get; set; }
    [JsonPropertyName("titleHidden")] public bool TitleHidden { get; set; }
    [JsonPropertyName("imageHidden")] public bool ImageHidden { get; set; }
}
