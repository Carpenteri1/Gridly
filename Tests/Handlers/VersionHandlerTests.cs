using Gridly.Command;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class VersionHandlerTests
{
    [Fact]
    public async Task HandleGetVersion_WhenCached_ReturnsCachedVersionWithoutCallingEndpoint()
    {
        var cache = new FakeMemoryCashingService();
        var cachedVersion = new VersionModel { Name = "1.0.0", NewRelease = false };
        cache.Seed("version", cachedVersion);
        var endPoint = new FakeVersionEndPoint();
        var handler = new VersionHandler(endPoint, cache);

        var result = await handler.Handle(new GetVersionCommand(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(cachedVersion.Name, payload.Name);
        Assert.Equal(0, endPoint.GetVersionCallCount);
    }

    [Fact]
    public async Task HandleGetVersion_WhenRemoteFetchSucceeds_StoresAndReturnsVersion()
    {
        var version = new VersionModel { Name = "2.0.0", NewRelease = true };
        var endPoint = new FakeVersionEndPoint { GetVersionResult = (true, version) };
        var cache = new FakeMemoryCashingService();
        var handler = new VersionHandler(endPoint, cache);

        var result = await handler.Handle(new GetVersionCommand(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(version.Name, payload.Name);
        Assert.Equal(1, endPoint.GetVersionCallCount);
        Assert.Equal(1, cache.StoreCallCount);
        Assert.Equal("version", cache.LastStoredKey);
        Assert.Same(version, cache.LastStoredValue);
    }

    [Fact]
    public async Task HandleGetVersion_WhenRemoteFetchFails_ReturnsNotFound()
    {
        var endPoint = new FakeVersionEndPoint { GetVersionResult = (false, null) };
        var cache = new FakeMemoryCashingService();
        var handler = new VersionHandler(endPoint, cache);

        var result = await handler.Handle(new GetVersionCommand(), CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
        Assert.Equal(1, endPoint.GetVersionCallCount);
        Assert.Equal(0, cache.StoreCallCount);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenRemoteFetchSucceeds_StoresAndReturnsVersion()
    {
        var version = new VersionModel { Name = "3.0.0", NewRelease = true };
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (true, version) };
        var cache = new FakeMemoryCashingService();
        var handler = new VersionHandler(endPoint, cache);

        var result = await handler.Handle(new GetLatestVersionCommand(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(version.Name, payload.Name);
        Assert.Equal(1, endPoint.GetLatestVersionCallCount);
        Assert.Equal(1, cache.StoreCallCount);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenRemoteFetchFails_ReturnsNotFound()
    {
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (false, null) };
        var cache = new FakeMemoryCashingService();
        var handler = new VersionHandler(endPoint, cache);

        var result = await handler.Handle(new GetLatestVersionCommand(), CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
        Assert.Equal(1, endPoint.GetLatestVersionCallCount);
        Assert.Equal(0, cache.StoreCallCount);
    }
}
