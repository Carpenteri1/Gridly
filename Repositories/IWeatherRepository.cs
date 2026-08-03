using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Repositories;

public interface IWeatherRepository
{
    public Task<(WeatherModel? Weather, DateTime? FetchedAt)> Get(string location);
    public Task<bool> Delete(int CardId);
    public Task<bool> Upsert(WeatherDataDtoModel weather);
}
