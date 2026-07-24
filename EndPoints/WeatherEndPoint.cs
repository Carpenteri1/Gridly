using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.EndPoints;

public class WeatherEndPoint(
    IDataConverter<WeatherDtoModel> dataConverter,
    IHttpClientServices httpClientServices) : IWeatherEndPoint
{
    public async Task<(bool, WeatherModel?)> Get(string location)
    {
        var APIKEY = string.Empty;
        if (string.IsNullOrEmpty(APIKEY)) return (false, null);
        //TODO add api key
        var url = string.Format(
            EndpointStrings.GetVisualCrossingWeatherData,
            Uri.EscapeDataString(location),
            APIKEY);

        var (success, jsonString) = await httpClientServices.Get(url);
        if (!success) return (false, null);

        var dto = dataConverter.DeserializeJson(jsonString);
        return dto is null ? (false, null) : (true, WeatherFactory.Create(dto));
    }
}
