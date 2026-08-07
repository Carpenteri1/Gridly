using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;

namespace Gridly.Repositories;

public class WeatherDataConnectionRepository(IDbConnection connection) : IWeatherDataConnectionRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<WeatherDataConnectionDtoModel> Upsert(WeatherDataConnectionDtoModel model)
    {
        return await _dbCommandRunner.Execute(QueryStrings.UpsertWeatherDataConnectionQuery, model);
    }

    public async Task<IEnumerable<WeatherDataConnectionDtoModel>> GetManyById(int? cardId, int? weatherId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectWeatherDataConnectionQuery);

        if (cardId != null)
            builder.Where(QueryStrings.WhereWeatherConnectedCardIdForeignKeyEqualIdWithAlias, new { CardId = cardId });
        if (weatherId != null)
            builder.Where(QueryStrings.WhereWeatherConnectedWeatherIdForeignKeyEqualIdWithAlias, new { WeatherId = weatherId });

        return await _dbCommandRunner.SelectMany<WeatherDataConnectionDtoModel>(template.RawSql, template.Parameters);
    }

    public async Task<bool> Delete(int cardId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.DeleteFromWeatherDataConnectionQuery);
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId, new { CardId = cardId });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters) != null;
    }
}
