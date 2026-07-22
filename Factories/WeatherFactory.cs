using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public class WeatherFactory
{
    public static WeatherModel Create(WeatherDtoModel dto)
        => new()
        {
            Location = dto.Name,
            Temperature = dto.Main.Temp,
            Description = dto.Weather.FirstOrDefault()?.Description ?? string.Empty,
        };
}
