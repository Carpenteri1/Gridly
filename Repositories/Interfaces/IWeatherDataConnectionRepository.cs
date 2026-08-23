using Gridly.Dtos;

namespace Gridly.Repositories.Interfaces;

public interface IWeatherDataConnectionRepository
{
    public Task<IEnumerable<WeatherDataConnectionDtoModel>> GetManyById(int? cardId, int? weatherId);
    public Task<WeatherDataConnectionDtoModel> Upsert(WeatherDataConnectionDtoModel model);
    public Task<bool> Delete(int cardId);
}
