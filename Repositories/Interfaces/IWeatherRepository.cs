using Gridly.Dtos;

namespace Gridly.Repositories.Interfaces;

public interface IWeatherRepository
{
    public Task<WeatherDataModel?> Get(string address);
    public Task<IEnumerable<StoredWeatherDataDto>?> GetStoredWeatherData();
    public Task<WeatherDataModel> Insert(WeatherDataModel weather);
}
