using Gridly.Constants;
using Gridly.Querys;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Services;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class VersionHandlerTests
{
    [Fact]
    public async Task HandleGetVersion_WhenNoCache_ReturnsNotFoundWithoutCallingEndpoint()
    {
        var cache = new FakeMemoryCashingService();
        var endPoint = new FakeVersionEndPoint();
        var appVersionProvider = new FakeAppVersionProvider();
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetVersionQuery(), CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
        Assert.Equal(0, endPoint.GetLatestVersionCallCount);
        Assert.Equal(0, appVersionProvider.CallCount);
    }

    [Fact]
    public async Task HandleGetVersion_WhenCached_ReturnsCachedVersionWithoutCallingGitHub()
    {
        var cache = new FakeMemoryCashingService();
        var cachedVersion = new VersionModel { Name = "1.0.0", NewRelease = false };
        cache.Seed(CacheKeyStrings.VersionCacheKey,
            new VersionCheckStateModel { Version = cachedVersion, CheckedAtUtc = DateTime.UtcNow });
        var endPoint = new FakeVersionEndPoint();
        var appVersionProvider = new FakeAppVersionProvider();
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetVersionQuery(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(cachedVersion.Name, payload.Name);
        Assert.Equal(0, endPoint.GetLatestVersionCallCount);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenCachedAndFresh_ReturnsCachedValueWithoutCallingGitHub()
    {
        var cache = new FakeMemoryCashingService();
        var cachedVersion = new VersionModel { Name = "1.0.0", NewRelease = false };
        cache.Seed(CacheKeyStrings.VersionCacheKey,
            new VersionCheckStateModel { Version = cachedVersion, CheckedAtUtc = DateTime.UtcNow.AddHours(-1) });
        var endPoint = new FakeVersionEndPoint();
        var appVersionProvider = new FakeAppVersionProvider();
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetLatestVersionQuery(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(cachedVersion.Name, payload.Name);
        Assert.Equal(0, endPoint.GetLatestVersionCallCount);
        Assert.Equal(0, appVersionProvider.CallCount);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenCacheStale_RefetchesFromGitHub()
    {
        var cache = new FakeMemoryCashingService();
        var staleVersion = new VersionModel { Name = "1.0.0", NewRelease = false };
        cache.Seed(CacheKeyStrings.VersionCacheKey,
            new VersionCheckStateModel { Version = staleVersion, CheckedAtUtc = DateTime.UtcNow.AddHours(-9) });
        var latestVersion = new VersionModel { Name = "2.0.0" };
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (true, latestVersion) };
        var appVersionProvider = new FakeAppVersionProvider { CurrentVersion = "1.0.0" };
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetLatestVersionQuery(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(1, endPoint.GetLatestVersionCallCount);
        Assert.Equal("2.0.0", payload.Name);
        Assert.True(payload.NewRelease);
        Assert.Equal(1, cache.StoreCallCount);
        Assert.NotNull(cache.LastStoredExpiration);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenGitHubFailsWithStaleCache_ReturnsStaleCachedValue()
    {
        var cache = new FakeMemoryCashingService();
        var staleVersion = new VersionModel { Name = "1.0.0", NewRelease = false };
        cache.Seed(CacheKeyStrings.VersionCacheKey,
            new VersionCheckStateModel { Version = staleVersion, CheckedAtUtc = DateTime.UtcNow.AddHours(-9) });
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (false, null) };
        var appVersionProvider = new FakeAppVersionProvider();
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetLatestVersionQuery(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(staleVersion.Name, payload.Name);
        Assert.Equal(0, cache.StoreCallCount);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenGitHubFailsWithNoCache_ReturnsNotFound()
    {
        var cache = new FakeMemoryCashingService();
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (false, null) };
        var appVersionProvider = new FakeAppVersionProvider();
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetLatestVersionQuery(), CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
        Assert.Equal(1, endPoint.GetLatestVersionCallCount);
        Assert.Equal(0, cache.StoreCallCount);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData("1.0.0", "v1.0.0", false)]
    [InlineData("V1.0.0", "v1.0.0", false)]
    [InlineData("1.0.0", "2.0.0", true)]
    public async Task HandleGetLatestVersion_ComparesCurrentVersionAgainstLatest(
        string currentVersion, string latestVersionName, bool expectedNewRelease)
    {
        var cache = new FakeMemoryCashingService();
        var latestVersion = new VersionModel { Name = latestVersionName };
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (true, latestVersion) };
        var appVersionProvider = new FakeAppVersionProvider { CurrentVersion = currentVersion };
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetLatestVersionQuery(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.Equal(expectedNewRelease, payload.NewRelease);
    }

    [Fact]
    public async Task HandleGetLatestVersion_WhenCurrentVersionIsDevSentinel_NeverReportsNewRelease()
    {
        var cache = new FakeMemoryCashingService();
        var latestVersion = new VersionModel { Name = "5.0.0" };
        var endPoint = new FakeVersionEndPoint { GetLatestVersionResult = (true, latestVersion) };
        var appVersionProvider = new FakeAppVersionProvider { CurrentVersion = AppVersionProvider.DevSentinelVersion };
        var handler = new VersionHandler(endPoint, cache, appVersionProvider);

        var result = await handler.Handle(new GetLatestVersionQuery(), CancellationToken.None);
        var payload = ResultAssertions.AssertOk<VersionModel>(result);

        Assert.False(payload.NewRelease);
    }
}
