using Gridly.Dtos;

namespace Gridly.EndPoints.Interfaces;

public interface IWeatherEndPoint
{
    public Task<(int, WeatherDataDto? Weather)> Get(string address, string rawKey);
}
