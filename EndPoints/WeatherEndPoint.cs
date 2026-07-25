using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;

namespace Gridly.EndPoints;

public class WeatherEndPoint(
    IDataConverter<WeatherDtoModel> dataConverter,
    IHttpClientServices httpClientServices,
    IProviderKeysRepository providerKeysRepository,
    IProviderKeysProtectionService providerKeysProtectionService) : IWeatherEndPoint
{
    public async Task<(WeatherFetchStatus Status, WeatherModel? Weather)> Get(string location)
    {
        var apiKey = await providerKeysRepository.Get(EndpointStrings.VisualCrossingProvider);
        if (apiKey is null) return (WeatherFetchStatus.NoApiKey, null);

        var rawKey = providerKeysProtectionService.Unprotect(apiKey.EncryptedKey);
        var url = string.Format(
            EndpointStrings.GetVisualCrossingWeatherData,
            Uri.EscapeDataString(location),
            rawKey);

        var (statusCode, body) = await httpClientServices.GetWithStatusCode(url);

        if (statusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden) return (WeatherFetchStatus.InvalidApiKey, null);
        if (statusCode is < StatusCodes.Status200OK or >= StatusCodes.Status300MultipleChoices) return (WeatherFetchStatus.ProviderUnavailable, null);

        var dto = dataConverter.DeserializeJson(body);
        return dto is null
            ? (WeatherFetchStatus.ProviderUnavailable, null)
            : (WeatherFetchStatus.Success, WeatherFactory.Create(dto));
    }
}
