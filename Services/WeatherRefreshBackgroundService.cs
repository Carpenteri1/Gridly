using Gridly.Commands;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Gridly.Services;

public class WeatherRefreshBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<WeatherRefreshBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    // Mirrors the "weather" rate-limiting policy (Configuration/RateLimiterPolicySettings) so this
    // background job never calls the provider faster than the HTTP endpoint is allowed to.
    private static readonly WeatherRateLimiterModel ProviderRate = new();
    internal static readonly TimeSpan DelayBetweenProviderCalls =
        ProviderRate.Window / ProviderRate.TokensPerPeriod;

    // Overridable seam so tests can assert on the delay without actually waiting on it.
    internal Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await RefreshAll(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    internal async Task RefreshAll(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var weatherRepository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

        var stored = await weatherRepository.GetStoredWeatherData();
        if (stored is null) return;

        var isFirst = true;
        foreach (var entry in stored)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!isFirst) await Delay(DelayBetweenProviderCalls, cancellationToken);
            isFirst = false;

            try
            {
                var result = await mediator.Send(
                    new GetVisualCrossingDataQuery { Address = entry.Address }, cancellationToken);

                if (result is Ok<WeatherDataModel> ok)
                {
                    ok.Value!.CardId = entry.CardId;
                    await mediator.Send(new SaveWeatherCommand { Weather = ok.Value }, cancellationToken);
                }
                else
                {
                    logger.LogWarning(
                        "Hourly weather refresh for {Address} did not return fresh data ({ResultType})",
                        entry.Address, result.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Hourly weather refresh failed for {Address}", entry.Address);
            }
        }
    }
}
