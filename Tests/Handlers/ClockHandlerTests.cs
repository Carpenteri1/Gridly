using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class ClockHandlerTests
{
    private static ClockModel MakeClock(string timeZone = "Europe/Stockholm") => new()
    {
        TimeZone = timeZone,
        UtcOffsetSeconds = 7200,
        DstActive = true,
    };

    [Fact]
    public async Task HandleGetClock_WhenCacheIsFresh_ReturnsCachedClock()
    {
        var repository = new FakeClockRepository();
        var clock = MakeClock();
        repository.Seed("Europe/Stockholm", clock, DateTime.UtcNow);
        var handler = new ClockHandler(new FakeClockEndPoint(), repository);

        var result = await handler.Handle(new GetClockQuery { SearchTerm = "Europe/Stockholm" }, CancellationToken.None);

        var returned = ResultAssertions.AssertOk<ClockModel>(result);
        Assert.Equal("Europe/Stockholm", returned.TimeZone);
    }

    [Fact]
    public async Task HandleGetClock_WhenCacheIsStale_ReturnsNotFound()
    {
        var repository = new FakeClockRepository();
        repository.Seed("Europe/Stockholm", MakeClock(), DateTime.UtcNow.AddHours(-2));
        var handler = new ClockHandler(new FakeClockEndPoint(), repository);

        var result = await handler.Handle(new GetClockQuery { SearchTerm = "Europe/Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetClock_WhenNothingStored_ReturnsNotFound()
    {
        var handler = new ClockHandler(new FakeClockEndPoint(), new FakeClockRepository());

        var result = await handler.Handle(new GetClockQuery { SearchTerm = "Europe/Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetClockData_WhenSuccessful_StoresAndReturnsClock()
    {
        var repository = new FakeClockRepository();
        var endpoint = new FakeClockEndPoint { Result = (ClockFetchStatus.Success, MakeClock()) };
        var handler = new ClockHandler(endpoint, repository);

        var result = await handler.Handle(new GetClockDataQuery { SearchTerm = "Europe/Stockholm" }, CancellationToken.None);

        var returned = ResultAssertions.AssertOk<ClockModel>(result);
        Assert.Equal("Europe/Stockholm", returned.TimeZone);
        Assert.Equal(1, repository.UpsertCallCount);
        Assert.Equal("Europe/Stockholm", repository.LastUpsertedLocation);
    }

    [Fact]
    public async Task HandleGetClockData_WhenProviderDownAndStaleDataExists_FallsBackToStaleClock()
    {
        var repository = new FakeClockRepository();
        repository.Seed("Europe/Stockholm", MakeClock(), DateTime.UtcNow.AddHours(-5));
        var endpoint = new FakeClockEndPoint { Result = (ClockFetchStatus.ProviderUnavailable, null) };
        var handler = new ClockHandler(endpoint, repository);

        var result = await handler.Handle(new GetClockDataQuery { SearchTerm = "Europe/Stockholm" }, CancellationToken.None);

        var returned = ResultAssertions.AssertOk<ClockModel>(result);
        Assert.Equal("Europe/Stockholm", returned.TimeZone);
        Assert.Equal(0, repository.UpsertCallCount);
    }

    [Fact]
    public async Task HandleGetClockData_WhenProviderDownAndNoStaleData_Returns503()
    {
        var endpoint = new FakeClockEndPoint { Result = (ClockFetchStatus.ProviderUnavailable, null) };
        var handler = new ClockHandler(endpoint, new FakeClockRepository());

        var result = await handler.Handle(new GetClockDataQuery { SearchTerm = "Europe/Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status503ServiceUnavailable);
    }
}
