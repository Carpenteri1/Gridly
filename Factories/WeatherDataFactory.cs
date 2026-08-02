using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public class WeatherDataFactory
{
    public static WeatherModel Create(WeatherDataDtoModel dto)
        => new()
        {
            CardId = dto.CardId,
            Location = dto.Location,
            Address = dto.Address,
            Timezone = dto.Timezone,
            Description = dto.Description,
            CurrentConditions = new CurrentConditionsModel
            {
                Conditions = dto.Conditions,
                Temp = dto.Temp,
                FeelsLike = dto.FeelsLike,
                Humidity = dto.Humidity,
                WindSpeed = dto.WindSpeed,
                WindDir = dto.WindDir,
            }
        };
}
