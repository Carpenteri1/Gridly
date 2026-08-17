using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.EndPoints;

public class ProvidersEndPointTests
{
    [Fact]
    public async Task Validate_CallsWeatherProviderWithProbeLocationAndKey_ReturnsStatusCode()
    {
        var providersEndPointExtensions = new FakeProvidersEndPointExtensions
        {
            Result = (StatusCodes.Status200OK, "{}")
        };
        var endPoint = new ProvidersEndPoint(providersEndPointExtensions);

        var status = await endPoint.Validate("raw-key");

        Assert.Equal(StatusCodes.Status200OK, status);
        Assert.Equal(EndpointStrings.VisualCrossingProbeLocation, providersEndPointExtensions.LastAddress);
        Assert.Equal(EndpointStrings.GetVisualCrossingWeatherData, providersEndPointExtensions.LastProvider);
        Assert.Equal("raw-key", providersEndPointExtensions.LastRawKey);
    }

    [Fact]
    public async Task Validate_WhenProviderRejectsKey_ReturnsProviderStatusCodeUnchanged()
    {
        var providersEndPointExtensions = new FakeProvidersEndPointExtensions
        {
            Result = (StatusCodes.Status401Unauthorized, "denied")
        };
        var endPoint = new ProvidersEndPoint(providersEndPointExtensions);

        var status = await endPoint.Validate("bad-key");

        Assert.Equal(StatusCodes.Status401Unauthorized, status);
    }
}
