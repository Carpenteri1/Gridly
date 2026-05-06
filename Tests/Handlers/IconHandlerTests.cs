using Gridly.Command;
using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

using SearchIconsHandler = global::CardHandler;

public class IconHandlerTests
{
    [Fact]
    public async Task Handle_WhenCacheHasIcons_FiltersWithoutCallingHttp()
    {
        var cache = new FakeMemoryCashingService();
        cache.Seed("mat-icons", new[] { "home e001", "grid_view e002", "search e003" });
        var httpClient = new FakeHttpClientServices();
        var handler = new SearchIconsHandler(httpClient, cache);

        var result = await handler.Handle(new SearchIconsCommand { Value = "GRID" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<SearchIconsResultDto>(result);

        Assert.Single(payload.Icons);
        Assert.Equal("grid_view e002", payload.Icons[0]);
        Assert.Equal(0, httpClient.CallCount);
    }

    [Fact]
    public async Task Handle_WhenCacheMisses_FetchesStoresAndReturnsMatches()
    {
        var cache = new FakeMemoryCashingService();
        var httpClient = new FakeHttpClientServices
        {
            Response = (true, "home e001\ngrid_view e002\nsearch e003")
        };
        var handler = new SearchIconsHandler(httpClient, cache);

        var result = await handler.Handle(new SearchIconsCommand { Value = "e00" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<SearchIconsResultDto>(result);

        Assert.Equal(3, payload.Icons.Length);
        Assert.Equal(1, httpClient.CallCount);
        Assert.Equal(EndpointStrings.materialIconsEndPoint, httpClient.LastUrl);
        Assert.Equal(1, cache.StoreCallCount);
    }

    [Fact]
    public async Task Handle_WhenRemotePayloadIsEmpty_ReturnsNoContent()
    {
        var cache = new FakeMemoryCashingService();
        var httpClient = new FakeHttpClientServices { Response = (true, string.Empty) };
        var handler = new SearchIconsHandler(httpClient, cache);

        var result = await handler.Handle(new SearchIconsCommand { Value = "grid" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status204NoContent);
        Assert.Equal(1, httpClient.CallCount);
        Assert.Equal(0, cache.StoreCallCount);
    }

    [Fact]
    public async Task Handle_LimitsResultsToFiftyMatches()
    {
        var icons = Enumerable.Range(1, 60).Select(i => $"grid_{i:00} e{i:000}").ToArray();
        var cache = new FakeMemoryCashingService();
        cache.Seed("mat-icons", icons);
        var httpClient = new FakeHttpClientServices();
        var handler = new SearchIconsHandler(httpClient, cache);

        var result = await handler.Handle(new SearchIconsCommand { Value = "grid_" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<SearchIconsResultDto>(result);

        Assert.Equal(50, payload.Icons.Length);
        Assert.Equal("grid_01 e001", payload.Icons[0]);
        Assert.Equal("grid_50 e050", payload.Icons[49]);
    }
}
