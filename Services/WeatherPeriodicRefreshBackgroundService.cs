using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.Factories;
using Gridly.Repositories;

namespace Gridly.Services;

public class WeatherPeriodicRefreshBackgroundService(
    ILogger<WeatherPeriodicRefreshBackgroundService> logger,
    IWeatherRepository weatherRepository,
    IWeatherEndPoint weatherEndPoint,
    ILocalProvidersRepository localProvidersRepository,
    IProviderKeysProtectionService providerKeysProtectionService) : PeriodicRefreshBackgroundService
{ 
    protected override TimeSpan Interval => TimeSpan.FromMinutes(30);
    protected override async Task RefreshData(CancellationToken cancellationToken)
    {
        var stored = await weatherRepository.GetStoredWeatherData();
        if (stored is null) return;

        var isFirst = true;
        foreach (var entry in stored)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!isFirst) await Delay(DelayBetweenProviderCalls, cancellationToken);
            isFirst = false;

            var storedKey = await localProvidersRepository.Get(EndpointStrings.VisualCrossingProvider);
            var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey);

            try
            {
                var (status, weather) = await weatherEndPoint.Get(entry.Address,rawKey);
                
                if (status is StatusCodes.Status200OK && weather is not null)
                {
                    await weatherRepository.Insert(WeatherDataFactory.Create(weather));
                    logger.LogInformation("Hourly weather refresh for {Address} completed", entry.Address);
                }
                else
                {
                    logger.LogWarning(
                        "Hourly weather refresh for {Address} did not return fresh data ({ResultType})",
                        entry.Address, status);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Hourly weather refresh failed for {Address}", entry.Address);
            }
        }
    }
}
