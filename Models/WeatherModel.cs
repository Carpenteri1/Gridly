using System.Text.Json.Serialization;

namespace Gridly.Models;

public class WeatherModel 
{ 
    [JsonPropertyName("resolvedAddress")] public string Location { get; set; } 
    [JsonPropertyName("address")] public string Address { get; set; }
    [JsonPropertyName("timezone")] public string Timezone { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("currentConditions")] public CurrentConditionsModel CurrentConditions { get; set; }
}