using Gridly.Models;

namespace Gridly.EndPoints;

public interface IWeatherEndPoint
{
    public Task<(int, WeatherModel? Weather)> Get(string location, string rawKey);
}
