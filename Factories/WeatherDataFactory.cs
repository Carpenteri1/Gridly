using Gridly.Dtos;

namespace Gridly.Factories;

public class WeatherDataFactory
{
    public static WeatherDataModel Create(WeatherDataDto dto)
    {
        var day = dto.days.FirstOrDefault()
            ?? throw new InvalidOperationException("Weather data contains no days.");
        return new()
        {
            Address = dto.address,
            Timezone = dto.timezone,
            Description = dto.description,
            Temp = day.temp,
            FeelsLike = day.feelslike,
            Humidity = day.humidity,
            WindSpeed = day.windspeed,
            WindDir = day.windDir,
            FetchedAt = dto.fetchedAt,
        };
    }
}