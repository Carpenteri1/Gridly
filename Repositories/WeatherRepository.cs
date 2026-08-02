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
        var dto = await _dbCommandRunner.Select<WeatherDataDtoModel>(
            QueryStrings.SelectWeatherDataQuery, new { Location = location });

        if (dto is null) return (null, null);

        return (WeatherDataFactory.Create(dto), dto.FetchedAt);
    }

    public async Task<bool> Delete(int CardId)
    {
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromWeatherQuery); 
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId, new { CardId});
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }

    public async Task<bool> Upsert(WeatherModel weather)
    {
        object parameters = new
        {
            CardId = weather.CardId,
            Location = weather.Location,
            Address = weather.Address,
            Timezone = weather.Timezone,
            Description = weather.Description,
            Conditions = weather.CurrentConditions.Conditions,
            Temp = weather.CurrentConditions.Temp,
            FeelsLike = weather.CurrentConditions.FeelsLike,
            Humidity = weather.CurrentConditions.Humidity,
            WindSpeed = weather.CurrentConditions.WindSpeed,
            WindDir = weather.CurrentConditions.WindDir,
            FetchedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpsertWeatherDataQuery, parameters);
    }
}
