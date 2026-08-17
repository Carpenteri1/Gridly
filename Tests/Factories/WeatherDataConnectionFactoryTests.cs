using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class WeatherDataConnectionFactoryTests
{
    [Fact]
    public void Create_MapsCardAndWeatherIds()
    {
        var result = WeatherDataConnectionFactory.Create(7, 15);

        Assert.Equal(7, result.CardId);
        Assert.Equal(15, result.WeatherId);
    }
}
