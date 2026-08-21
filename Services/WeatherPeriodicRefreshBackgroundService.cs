using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.EndPoints.Interfaces;
using Gridly.Enums;
using Gridly.Factories;
using Gridly.Repositories;
using Gridly.Repositories.Interfaces;

namespace Gridly.Services;

public class WeatherPeriodicRefreshBackgroundService(
    ILogger<WeatherPeriodicRefreshBackgroundService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IProviderKeysProtectionService providerKeysProtectionService) : PeriodicRefreshBackgroundService
{
    protected override TimeSpan Interval => TimeSpan.FromHours(1);

    protected override async Task RefreshData(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();//Creates temperary dependency injection scope
        var weatherRepository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();
        var weatherEndPoint = scope.ServiceProvider.GetRequiredService<IWeatherEndPoint>();
        var localProvidersRepository = scope.ServiceProvider.GetRequiredService<ILocalProvidersRepository>();

        var stored = await weatherRepository.GetStoredWeatherData();
        if (stored is null) return;

        var storedKey = await localProvidersRepository.Get(EndpointStrings.VisualCrossingProvider);
        if (storedKey is null)
        {
            logger.LogWarning("Hourly weather refresh skipped because no Visual Crossing provider key is stored");
            return;
        }

        var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey);
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            logger.LogWarning("Hourly weather refresh skipped because no Valid Visual Crossing provider key is stored");
            return;
        }
        
        var isFirstCall = true;
        
        foreach (var entry in stored)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!isFirstCall) await Delay(DelayBetweenProviderCalls, cancellationToken);

            isFirstCall = false;
            try
            {
                var (status, weather) = await weatherEndPoint.Get(entry.Address, rawKey);
                if (status is StatusCodes.Status200OK && weather is not null)
                {
                    await weatherRepository.Insert(WeatherDataFactory.Create(weather));
                    await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider,
                        nameof(ProvidersKeyStatusEnum.Valid));

                    logger.LogInformation("Hourly weather refresh for {Address} completed", entry.Address);
                }
            }
            catch (HttpRequestException ex)
            {
                var s = ex.StatusCode;
                logger.LogWarning(
                    "Hourly weather refresh for {Address} did not return fresh data",
                    entry.Address);
                await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider,
                    nameof(ProvidersKeyStatusEnum.Invalid));
            }
            catch (Exception)
            {
                logger.LogWarning(
                    "Hourly weather refresh for {Address} did not return fresh data",
                    entry.Address);
            }
        }
    }
}
