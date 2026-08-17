using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;

namespace Gridly.Repositories;

public class WeatherRepository(IDbConnection connection) : IWeatherRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<WeatherDataModel?> Get(string address)
    {
        var builder = new SqlBuilder();
        builder.Where(QueryStrings.WhereLocationEqualsLocation);
        var template = builder.AddTemplate(QueryStrings.SelectWeatherDataQuery); 
        
        var dto = await _dbCommandRunner.Select<WeatherDataModel>(
            template.RawSql, new { Address = address });
        
        return dto;
    }

    public async Task<IEnumerable<StoredWeatherDataDto>?> GetStoredWeatherData()
    {
        var storedWeatherData =
            await _dbCommandRunner.SelectMany<StoredWeatherDataDto>(QueryStrings.SelectAllWeatherDataQuery, string.Empty);
        return storedWeatherData;
    }

    public async Task<WeatherDataModel> Upsert(WeatherDataModel weather) =>
        await _dbCommandRunner.Execute(QueryStrings.UpsertWeatherDataQuery, weather);
}
