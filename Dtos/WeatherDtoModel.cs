using System.Text.Json.Serialization;

namespace Gridly.Dtos;

public class WeatherDtoModel
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("main")] public WeatherMainDto Main { get; set; }
    [JsonPropertyName("weather")] public List<WeatherConditionDto> Weather { get; set; }
}

public class WeatherMainDto
{
    [JsonPropertyName("temp")] public double Temp { get; set; }
}

public class WeatherConditionDto
{
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("icon")] public string Icon { get; set; }
}
