using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.Models;

namespace Gridly.Repositories;

public class WeatherRepository(IDbConnection connection) : IWeatherRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<(WeatherModel? Weather, DateTime? FetchedAt)> Get(string location)
    {
        var builder = new SqlBuilder();
        builder.Where(QueryStrings.WhereLocationEqualsLocation);
        var template = builder.AddTemplate(QueryStrings.SelectWeatherDataQuery); 
        
        var dto = await _dbCommandRunner.Select<WeatherDataDtoModel>(
            template.RawSql, new { Location = location });

        if (dto is null) return (null, null);

        return (WeatherDataFactory.Create(dto), dto.FetchedAt);
    }

    public async Task<(WeatherModel? Weather, DateTime? FetchedAt)> GetById(int cardId)
    {
        var builder = new SqlBuilder();
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId);
        var template = builder.AddTemplate(QueryStrings.SelectWeatherDataQuery); 
        
        var dto = await _dbCommandRunner.Select<WeatherDataDtoModel>(
            template.RawSql, new { CardId = cardId });

        if (dto is null) return (null, null);

        return (WeatherDataFactory.Create(dto), dto.FetchedAt);
    }

    public async Task<bool> Delete(int cardId)
    {
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromWeatherQuery); 
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId, new { cardId});
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }

    public async Task<bool> Upsert(WeatherDataDtoModel weather) => 
        await _dbCommandRunner.Execute(QueryStrings.UpsertWeatherDataQuery, weather as object);
}
