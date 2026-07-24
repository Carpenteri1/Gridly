using System.Text.Json.Serialization;

namespace Gridly.Dtos;

public class CurrentConditionsDtoModel
{ 
    [JsonPropertyName("conditions")] public string Conditions { get; set; }
    [JsonPropertyName("temp")] public double Temp { get; set; }
    [JsonPropertyName("feelslike")] public double FeelsLike { get; set; }
    [JsonPropertyName("humidity")] public double Humidity { get; set; } 
    [JsonPropertyName("windspeed")] public double WindSpeed { get; set; }
    [JsonPropertyName("winddir")] public double WindDir { get; set; }
}