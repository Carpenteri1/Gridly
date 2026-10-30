using Gridly.Constants;
using Gridly.Data;
using Gridly.EndPoints.Interfaces;
using Gridly.Enums;
using Gridly.Extension;
using Gridly.Factories;

namespace Gridly.Services;

public class WeatherPeriodicRefreshBackgroundService(
    GridlyDbContext dbContext,
    ILogger<WeatherPeriodicRefreshBackgroundService> logger,
    IProviderKeysProtectionService providerKeysProtectionService,
    IWeatherEndPoint weatherEndPoint) : PeriodicRefreshBackgroundService
{
    protected override TimeSpan Interval => TimeSpan.FromHours(1);

    protected override async Task RefreshData(CancellationToken cancellationToken)
    {
        var weatherDataEntities = dbContext.WeatherData.AsQueryable();
        if (weatherDataEntities is null) return;

        var storedKey = await dbContext.ProviderKeys.GetLocalProviderKeyByProvider(EndpointStrings.VisualCrossingProvider);
        if (storedKey is null)
        {
            logger.LogWarning("Hourly weather refresh skipped because no Visual Crossing provider key is stored");
            return;
        }

        var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey!);
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            logger.LogWarning("Hourly weather refresh skipped because no Valid Visual Crossing provider key is stored");
            return;
        }
        
        var isFirstCall = true;
        
        foreach (var entry in weatherDataEntities)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!isFirstCall) await Delay(DelayBetweenProviderCalls, cancellationToken);

            isFirstCall = false;
            try
            {
                var (status, weather) = await weatherEndPoint.Get(entry.Address, rawKey);
                if (status is StatusCodes.Status200OK && weather is not null)
                {
                    var convertedWeatherDataEntities = WeatherDataFactory.Create(weather);
                    dbContext.WeatherData.Add(convertedWeatherDataEntities);
                    
                    var localProviderEntity = await dbContext.ProviderKeys.GetLocalProviderKeyByProvider(EndpointStrings.VisualCrossingProvider);
                    localProviderEntity!.Status = nameof(ProvidersKeyStatusEnum.Valid);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    logger.LogInformation("Hourly weather refresh for {Address} completed", entry.Address);
                }
            }
            catch (HttpRequestException ex)
            {
                var s = ex.StatusCode;
                logger.LogWarning(
                    "Hourly weather refresh for {Address} did not return fresh data, status code {StatusCode}",
                    entry.Address, ex.StatusCode);
                var localProviderEntity = await dbContext.ProviderKeys.GetLocalProviderKeyByProvider(EndpointStrings.VisualCrossingProvider);
                localProviderEntity!.Status = nameof(ProvidersKeyStatusEnum.Valid);

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
