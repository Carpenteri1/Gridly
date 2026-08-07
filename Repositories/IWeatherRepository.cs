using Gridly.Dtos;

namespace Gridly.Repositories;

public interface IWeatherRepository
{
    public Task<WeatherDataModel> Get(string address);
    public Task<WeatherDataModel> GetById(int cardId);
    public Task<IEnumerable<WeatherDataModel>?> GetStoredWeatherData();
    public Task<bool> Update(WeatherDataModel weather);
    public Task<bool> Insert(WeatherDataModel weather);
}
