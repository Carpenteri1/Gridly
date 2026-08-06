using Gridly.Constants;
using Gridly.Dtos;
using Gridly.helpers;
using Gridly.Services;

namespace Gridly.EndPoints;

public class WeatherEndPoint(
    IDataConverter<WeatherDataDto> dataConverter,
    IProvidersEndPointExtensions providersEndPointExtensions) : IWeatherEndPoint
{
    public async Task<(int, WeatherDataDto? Weather)> Get(string address, string rawKey)
    {
        var (status, body) = await providersEndPointExtensions.CallWeatherProvider(
            address,
            EndpointStrings.GetVisualCrossingWeatherData,
            rawKey);

        var dto = dataConverter.DeserializeJson(body);
        return (status, dto);
    }
}
