using Gridly.Dtos;
using Gridly.Entities;

namespace Gridly.Factories;

public class WeatherDataFactory
{
    public static WeatherDataModel Create(WeatherDataEntity entity) => new()
    {
        Id = entity.Id,
        Address = entity.Address,
        Timezone = entity.Timezone,
        Description = entity.Description,
        Temp = entity.Temp,
        FeelsLike = entity.FeelsLike,
        Humidity = entity.Humidity,
        WindSpeed = entity.WindSpeed,
        WindDir = entity.WindDir,
        FetchedAt = entity.FetchedAt,
    };

    public static WeatherDataModel Create(WeatherDataDto dto)
    {
        var day = dto.days.FirstOrDefault();
        return new()
        {
            Address = dto.address,
            Timezone = dto.timezone,
            Description = dto.description,
            Temp = day!.temp,
            FeelsLike = day!.feelslike,
            Humidity = day!.humidity,
            WindSpeed = day!.windspeed,
            WindDir = day!.windDir,
            FetchedAt = dto.fetchedAt,
        };
    }
}