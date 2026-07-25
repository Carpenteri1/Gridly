using Gridly.Models;

namespace Gridly.EndPoints;

public interface IWeatherEndPoint
{
    public Task<(WeatherFetchStatus Status, WeatherModel? Weather)> Get(string location);
}
