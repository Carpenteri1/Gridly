using Gridly.helpers;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Helpers;

public class WeatherProviderExtensionsTests
{
    [Fact]
    public async Task CallWeatherProvider_EscapesLocationInRequestUrl()
    {
        var httpClient = new FakeHttpClientServices
        {
            StatusCodeResponse = (StatusCodes.Status200OK, "{}")
        };
        var provider = new ProvidersEndPointExtensions(httpClient);

        await provider.CallWeatherProvider("New York", "https://weather.example/{0}?key={1}", "test-key");

        Assert.NotNull(httpClient.LastUrl);
        Assert.Contains("New%20York", httpClient.LastUrl);
        Assert.Contains("test-key", httpClient.LastUrl);
        Assert.DoesNotContain(" ", httpClient.LastUrl);
    }

    [Fact]
    public async Task CallWeatherProvider_ReturnsMappedStatusAndBody()
    {
        var httpClient = new FakeHttpClientServices
        {
            StatusCodeResponse = (StatusCodes.Status401Unauthorized, "denied")
        };
        var provider = new ProvidersEndPointExtensions(httpClient);

        var (status, body) = await provider.CallWeatherProvider("London", "https://weather.example/{0}?key={1}", "bad-key");

        Assert.Equal(StatusCodes.Status401Unauthorized, status);
        Assert.Equal("denied", body);
    }
}
