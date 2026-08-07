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

    public async Task<WeatherDataModel> GetById(int cardId)
    {
        var builder = new SqlBuilder();
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId);
        var template = builder.AddTemplate(QueryStrings.SelectWeatherDataQuery); 
        
        var weather = await _dbCommandRunner.Select<WeatherDataModel>(
            template.RawSql, new { CardId = cardId });
        
        return weather;
    }

    public async Task<IEnumerable<WeatherDataModel>?> GetStoredWeatherData()
    {
        var storedWeatherData =
            await _dbCommandRunner.SelectMany<WeatherDataModel>(QueryStrings.SelectWeatherDataQuery, string.Empty);
        return storedWeatherData;    
    }

    public async Task<bool> Update(WeatherDataModel weather) => 
        await _dbCommandRunner.Execute(QueryStrings.UpdateWeatherDataQuery, weather as object);
    
    public async Task<bool> Insert(WeatherDataModel weather) => 
        await _dbCommandRunner.Execute(QueryStrings.InsertWeatherDataQuery, weather as object);
}
