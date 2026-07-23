using System.Text.Json.Serialization;
using Gridly.Models;

namespace Gridly.Dtos;

public class WeatherDtoModel
{ 
    [JsonPropertyName("resolvedAddress")] public string Location { get; set; } 
    [JsonPropertyName("address")] public string Address { get; set; }
    [JsonPropertyName("timezone")] public string Timezone { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("currentConditions")] public CurrentConditionsDtoModel CurrentConditions { get; set; }
}
