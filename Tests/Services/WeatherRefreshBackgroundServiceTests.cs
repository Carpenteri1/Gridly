using Gridly.Commands;
using Gridly.Dtos;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Tests.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gridly.Tests.Services;

public class WeatherRefreshBackgroundServiceTests
{
    private static WeatherRefreshBackgroundService BuildService(
        FakeMediator mediator, FakeWeatherRepository repository)
    {
        var provider = new ServiceCollection()
            .AddSingleton<IMediator>(mediator)
            .AddSingleton<IWeatherRepository>(repository)
            .BuildServiceProvider();

        return new WeatherRefreshBackgroundService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<WeatherRefreshBackgroundService>.Instance);
    }

    [Fact]
    public async Task RefreshAll_WithNoStoredWeather_DoesNotCallProvider()
    {
        var mediator = new FakeMediator();
        var repository = new FakeWeatherRepository();
        var service = BuildService(mediator, repository);

        await service.RefreshAll(CancellationToken.None);

        Assert.Empty(mediator.SentRequests);
    }

    [Fact]
    public async Task RefreshAll_ForStoredAddress_FetchesFreshDataAndSavesWithCardId()
    {
        var stored = new WeatherDataModel { CardId = 7, Address = "Stockholm" };
        var repository = new FakeWeatherRepository { StoredWeatherData = [stored] };
        var mediator = new FakeMediator
        {
            OnGetVisualCrossingData = query => Results.Ok(new WeatherDataModel
            {
                Address = query.Address,
                Description = "clear",
                FetchedAt = DateTime.UtcNow
            })
        };
        var service = BuildService(mediator, repository);

        await service.RefreshAll(CancellationToken.None);

        var saveCommand = mediator.SentRequests.OfType<SaveWeatherCommand>().Single();
        Assert.Equal(7, saveCommand.Weather.CardId);
        Assert.Equal("Stockholm", saveCommand.Weather.Address);
    }

    [Fact]
    public async Task RefreshAll_WhenProviderCallIsNotOk_DoesNotSave()
    {
        var stored = new WeatherDataModel { CardId = 1, Address = "Stockholm" };
        var repository = new FakeWeatherRepository { StoredWeatherData = [stored] };
        var mediator = new FakeMediator { OnGetVisualCrossingData = _ => Results.Unauthorized() };
        var service = BuildService(mediator, repository);

        await service.RefreshAll(CancellationToken.None);

        Assert.Empty(mediator.SentRequests.OfType<SaveWeatherCommand>());
    }

    [Fact]
    public async Task RefreshAll_WhenOneAddressThrows_StillProcessesTheRest()
    {
        var first = new WeatherDataModel { CardId = 1, Address = "Stockholm" };
        var second = new WeatherDataModel { CardId = 2, Address = "Oslo" };
        var repository = new FakeWeatherRepository { StoredWeatherData = [first, second] };
        var mediator = new FakeMediator
        {
            OnGetVisualCrossingData = query => query.Address == "Stockholm"
                ? throw new InvalidOperationException("provider unavailable")
                : Results.Ok(new WeatherDataModel { Address = query.Address })
        };
        var service = BuildService(mediator, repository);
        service.Delay = (_, _) => Task.CompletedTask;

        await service.RefreshAll(CancellationToken.None);

        var saveCommand = mediator.SentRequests.OfType<SaveWeatherCommand>().Single();
        Assert.Equal(2, saveCommand.Weather.CardId);
    }

    [Fact]
    public async Task RefreshAll_WithMultipleAddresses_DelaysBetweenCallsButNotBeforeTheFirst()
    {
        WeatherDataModel[] entries =
        [
            new() { CardId = 1, Address = "Stockholm" },
            new() { CardId = 2, Address = "Oslo" },
            new() { CardId = 3, Address = "Helsinki" }
        ];
        var repository = new FakeWeatherRepository { StoredWeatherData = entries };
        var mediator = new FakeMediator
        {
            OnGetVisualCrossingData = query => Results.Ok(new WeatherDataModel { Address = query.Address })
        };
        var service = BuildService(mediator, repository);
        var delays = new List<TimeSpan>();
        service.Delay = (delay, _) =>
        {
            delays.Add(delay);
            return Task.CompletedTask;
        };

        await service.RefreshAll(CancellationToken.None);

        Assert.Equal(2, delays.Count);
        Assert.All(delays, d => Assert.Equal(WeatherRefreshBackgroundService.DelayBetweenProviderCalls, d));
    }
}
