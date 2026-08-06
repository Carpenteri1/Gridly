using Gridly.Dtos;

namespace Gridly.EndPoints;

public interface IWeatherEndPoint
{
    public Task<(int, WeatherDataDto? Weather)> Get(string address, string rawKey);
}
