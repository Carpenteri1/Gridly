using System.Data;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Factories;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class WeatherRepository(IDbConnection connection, GridlyDbContext dbContext) : IWeatherRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<WeatherDataModel?> Get(string address)
    {
        var entity = await dbContext.WeatherData.AsNoTracking()
            .SingleOrDefaultAsync(w => w.Address == address);
        return entity is null ? null : WeatherDataFactory.Create(entity);
    }

    public async Task<IEnumerable<StoredWeatherDataDto>?> GetStoredWeatherData()
    {
        var query =
            from wc in dbContext.WeatherDataConnections.AsNoTracking()
            join w in dbContext.WeatherData.AsNoTracking() on wc.WeatherId equals w.Id
            select new StoredWeatherDataDto
            {
                Id = w.Id,
                CardId = wc.CardId,
                Address = w.Address,
                Timezone = w.Timezone,
                Description = w.Description,
                Temp = w.Temp,
                FeelsLike = w.FeelsLike,
                Humidity = w.Humidity,
                WindSpeed = w.WindSpeed,
                WindDir = w.WindDir,
                FetchedAt = w.FetchedAt,
            };
        return await query.ToListAsync();
    }

    public async Task<WeatherDataModel> Upsert(WeatherDataModel weather) =>
        await _dbCommandRunner.Execute(QueryStrings.UpsertWeatherDataQuery, weather);
}
