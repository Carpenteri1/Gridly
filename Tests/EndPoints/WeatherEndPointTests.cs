using Gridly.Constants;
using Gridly.Dtos;
using Gridly.EndPoints;
using Gridly.Services;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.EndPoints;

public class WeatherEndPointTests
{
    [Fact]
    public async Task Get_WhenStatusIsOk_DeserializesWeatherData()
    {
        const string json = """{"address":"London","timezone":"Europe/London","description":"Sunny","days":[{"temp":20,"feelslike":19,"humidity":50,"windspeed":4,"windDir":90}],"fetchedAt":"2026-08-17T00:00:00"}""";
        var providersEndPointExtensions = new FakeProvidersEndPointExtensions
        {
            Result = (StatusCodes.Status200OK, json)
        };
        var endPoint = new WeatherEndPoint(new DataConverter<WeatherDataDto>(), providersEndPointExtensions);

        var (status, weather) = await endPoint.Get("London", "raw-key");

        Assert.Equal(StatusCodes.Status200OK, status);
        Assert.NotNull(weather);
        Assert.Equal("London", weather.address);
        Assert.Equal("raw-key", providersEndPointExtensions.LastRawKey);
        Assert.Equal(EndpointStrings.GetVisualCrossingWeatherData, providersEndPointExtensions.LastProvider);
    }

    [Fact]
    public async Task Get_WhenStatusIsNotOk_ThrowsHttpRequestException()
    {
        var providersEndPointExtensions = new FakeProvidersEndPointExtensions
        {
            Result = (StatusCodes.Status401Unauthorized, "denied")
        };
        var endPoint = new WeatherEndPoint(new DataConverter<WeatherDataDto>(), providersEndPointExtensions);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => endPoint.Get("London", "raw-key"));
        Assert.Contains("401", exception.Message);
    }
}
