using System.Data;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class WeatherRepository(IDbConnection connection, IDataConverter<WeatherModel> dataConverter) : IWeatherRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<(WeatherModel? Weather, DateTime? FetchedAt)> Get(string location)
    {
        var dto = await _dbCommandRunner.Select<WeatherDataDtoModel>(
            QueryStrings.SelectWeatherDataQuery, new { Location = location });

        if (dto is null) return (null, null);

        return (dataConverter.DeserializeJson(dto.JsonPayload), dto.FetchedAt);
    }

    public async Task<bool> Upsert(string location, WeatherModel weather)
    {
        object parameters = new
        {
            Location = location,
            JsonPayload = dataConverter.SerializerToJsonString(weather),
            FetchedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpsertWeatherDataQuery, parameters);
    }
}
