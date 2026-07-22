using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.EndPoints;

public class WeatherEndPoint(
    IDataConverter<WeatherDtoModel> dataConverter,
    IHttpClientServices httpClientServices,
    WeatherApiOptions weatherApiOptions) : IWeatherEndPoint
{
    public async Task<(bool, WeatherModel?)> Get(string location)
    {
        var url = string.Format(
            EndpointStrings.GetWeatherRemoteEndPoint,
            Uri.EscapeDataString(location),
            weatherApiOptions.ApiKey);

        var (success, item) = await httpClientServices.Get(url);
        if (!success) return (false, null);

        var dto = dataConverter.DeserializeJson(item);
        return dto is null ? (false, null) : (true, WeatherFactory.Create(dto));
    }
}
