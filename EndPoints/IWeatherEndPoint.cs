using Gridly.Models;

namespace Gridly.EndPoints;

public interface IWeatherEndPoint
{
    public Task<(bool, WeatherModel?)> Get(string location);
}