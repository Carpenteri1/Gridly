using Gridly.Commands;
using Gridly.Dtos;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await RefreshAll(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RefreshAll(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var weatherRepository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

        var stored = await weatherRepository.GetStoredWeatherData();
        if (stored is null) return;

        foreach (var entry in stored)
        {
            if (cancellationToken.IsCancellationRequested) return;

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
