using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class WeatherFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToWeatherModel()
    {
        var weatherDto = new WeatherDtoModel
        {
            Location = "Stockholm",
            Timezone = "Europe/Stockholm",
            Address = "Stockholm,Sweden",
            Description = "clear sky",
        };

        weatherDto.CurrentConditions = new CurrentConditionsDtoModel
        {
            Conditions = "clear sky",
            Temp = 21.5,
            FeelsLike = 21.5,
            Humidity = 50,
            WindSpeed = 1.5,
            WindDir = 180,
        };
        
        var result = WeatherFactory.Create(weatherDto);

        Assert.Equal("Stockholm", result.Location);
        Assert.Equal(21.5, result.CurrentConditions.Temp);
        Assert.Equal("clear sky", result.Description);
    }
}
