using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Factories;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class WeatherRepository(GridlyDbContext dbContext) : IWeatherRepository
{
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

    public async Task<WeatherDataModel> Insert(WeatherDataModel weather)
    {
        var entity = new WeatherDataEntity
        {
            Address = weather.Address,
            Timezone = weather.Timezone,
            Description = weather.Description,
            Temp = weather.Temp,
            FeelsLike = weather.FeelsLike,
            Humidity = weather.Humidity,
            WindSpeed = weather.WindSpeed,
            WindDir = weather.WindDir,
            FetchedAt = weather.FetchedAt,
        };
        
        dbContext.WeatherData.Add(entity);
        await dbContext.SaveChangesAsync();
        return WeatherDataFactory.Create(entity);
    }
}
