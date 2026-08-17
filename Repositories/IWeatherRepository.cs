using Gridly.Dtos;

namespace Gridly.Repositories;

public interface IWeatherRepository
{
    public Task<WeatherDataModel?> Get(string address);
    public Task<IEnumerable<StoredWeatherDataDto>?> GetStoredWeatherData();
    public Task<WeatherDataModel> Upsert(WeatherDataModel weather);
}
