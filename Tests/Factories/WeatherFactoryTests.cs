using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class WeatherFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToWeatherModel()
    {
        var dto = new WeatherDtoModel
        {
            Name = "Stockholm",
            Main = new WeatherMainDto { Temp = 21.5 },
            Weather = [new WeatherConditionDto { Description = "clear sky", Icon = "01d" }]
        };

        var result = WeatherFactory.Create(dto);

        Assert.Equal("Stockholm", result.Location);
        Assert.Equal(21.5, result.Temperature);
        Assert.Equal("clear sky", result.Description);
    }

    [Fact]
    public void Create_WhenWeatherConditionsListIsEmpty_DefaultsDescriptionAndIconToEmptyString()
    {
        var dto = new WeatherDtoModel
        {
            Name = "Malmo",
            Main = new WeatherMainDto { Temp = 12.3 },
            Weather = []
        };

        var result = WeatherFactory.Create(dto);

        Assert.Equal(string.Empty, result.Description);
    }
}
