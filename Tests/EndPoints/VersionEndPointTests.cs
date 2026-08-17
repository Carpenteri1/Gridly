using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.EndPoints;

public class VersionEndPointTests
{
    [Fact]
    public async Task GetVersion_WhenSuccessful_ReturnsDeserializedVersion()
    {
        var httpClient = new FakeHttpClientServices
        {
            Response = (true, """{"name":"1.2.3","newRelease":false}""")
        };
        var endPoint = new VersionEndPoint(new DataConverter<VersionModel>(), httpClient);

        var (success, version) = await endPoint.GetVersion();

        Assert.True(success);
        Assert.NotNull(version);
        Assert.Equal("1.2.3", version.Name);
        Assert.Equal(EndpointStrings.GetVersionInternalEndPoint, httpClient.LastUrl);
    }

    [Fact]
    public async Task GetVersion_WhenUnsuccessful_ReturnsNullVersion()
    {
        var httpClient = new FakeHttpClientServices { Response = (false, string.Empty) };
        var endPoint = new VersionEndPoint(new DataConverter<VersionModel>(), httpClient);

        var (success, version) = await endPoint.GetVersion();

        Assert.False(success);
        Assert.Null(version);
    }

    [Fact]
    public async Task GetLatestVersion_WhenSuccessful_CallsRemoteEndpointAndReturnsVersion()
    {
        var httpClient = new FakeHttpClientServices
        {
            Response = (true, """{"name":"2.0.0","newRelease":true}""")
        };
        var endPoint = new VersionEndPoint(new DataConverter<VersionModel>(), httpClient);

        var (success, version) = await endPoint.GetLatestVersion();

        Assert.True(success);
        Assert.NotNull(version);
        Assert.True(version.NewRelease);
        Assert.Equal(EndpointStrings.GetVersionRemoteEndPoint, httpClient.LastUrl);
    }
}
