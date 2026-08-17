using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class WeatherDataFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsFromFirstDay()
    {
        var dto = new WeatherDataDto(
            address: "Stockholm",
            timezone: "Europe/Stockholm",
            description: "Cloudy",
            days: new[]
            {
                new DaysDto(temp: 12.5, feelslike: 11.0, humidity: 80, windspeed: 5.5, windDir: 180),
                new DaysDto(temp: 20.0, feelslike: 19.0, humidity: 60, windspeed: 3.0, windDir: 90)
            },
            fetchedAt: new DateTime(2026, 8, 17));

        var result = WeatherDataFactory.Create(dto);

        Assert.Equal("Stockholm", result.Address);
        Assert.Equal("Europe/Stockholm", result.Timezone);
        Assert.Equal("Cloudy", result.Description);
        Assert.Equal(12.5, result.Temp);
        Assert.Equal(11.0, result.FeelsLike);
        Assert.Equal(80, result.Humidity);
        Assert.Equal(5.5, result.WindSpeed);
        Assert.Equal(180, result.WindDir);
        Assert.Equal(dto.fetchedAt, result.FetchedAt);
    }

    [Fact]
    public void Create_WithNoDays_ThrowsInvalidOperationException()
    {
        var dto = new WeatherDataDto(
            address: "Stockholm",
            timezone: "Europe/Stockholm",
            description: "Cloudy",
            days: Array.Empty<DaysDto>(),
            fetchedAt: DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => WeatherDataFactory.Create(dto));
    }
}
