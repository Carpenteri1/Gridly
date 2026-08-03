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
    public static WeatherDataDtoModel CreateDto(WeatherModel weather)
        => new()
        {
            CardId = weather.CardId,
            Location = weather.Location,
            Address = weather.Address,
            Timezone = weather.Timezone,
            Description = weather.Description,
            Conditions = weather.CurrentConditions.Conditions,
            Temp = weather.CurrentConditions.Temp,
            FeelsLike = weather.CurrentConditions.FeelsLike,
            Humidity = weather.CurrentConditions.Humidity,
            WindSpeed = weather.CurrentConditions.WindSpeed,
            WindDir = weather.CurrentConditions.WindDir,
        };
}
