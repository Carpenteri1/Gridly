using Gridly.Dtos;
using Gridly.Factories;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.EndPoints;

public class WeatherEndPoint(
    IDataConverter<WeatherDtoModel> dataConverter,
    IProvidersEndPointExtensions providersEndPointExtensions) : IWeatherEndPoint
{
    public async Task<(int, WeatherModel? Weather)> Get(string location,string provider, string rawKey)
    {
        var (status, body) = await providersEndPointExtensions.CallWeatherProvider(location, provider, rawKey);

        var dto = dataConverter.DeserializeJson(body);

        return dto is null
            ? (status, null)
            : (status, WeatherFactory.Create(dto));
    }
}
