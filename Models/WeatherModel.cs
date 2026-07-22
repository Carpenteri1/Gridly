using System.Text.Json.Serialization;

namespace Gridly.Models;

public class WeatherModel
{
    [JsonPropertyName("location")] public string Location { get; set; }
    [JsonPropertyName("temperature")] public double Temperature { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
}