using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public class WeatherFactory
{
    public static WeatherModel Create(WeatherDtoModel dto)
        => new()
        {
            Location = dto.Location,
            Address = dto.Address,
            Timezone = dto.Timezone,
            Description = dto.Description,
            CurrentConditions = CurrentConditionsFactory.Create(dto.CurrentConditions),
        };
}