using Gridly.Models;

namespace Gridly.Repositories;

public interface IWeatherRepository
{
    public Task<(WeatherModel? Weather, DateTime? FetchedAt)> Get(string location);
    public Task<bool> Upsert(string location, WeatherModel weather);
}
