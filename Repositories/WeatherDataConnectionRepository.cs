using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Factories;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class WeatherDataConnectionRepository(GridlyDbContext dbContext) : IWeatherDataConnectionRepository
{
    public async Task<WeatherDataConnectionDtoModel> Upsert(WeatherDataConnectionDtoModel model)
    {
        var entity = await dbContext.WeatherDataConnections
            .FirstOrDefaultAsync(w => w.CardId == model.CardId);

        if (entity is not null)
        {
            entity.WeatherId = model.WeatherId!.Value;
        }
        else
        {
            entity = new WeatherDataConnectionEntity
            {
                CardId = model.CardId!.Value,
                WeatherId = model.WeatherId!.Value,
            };
            dbContext.WeatherDataConnections.Add(entity);
        }

        await dbContext.SaveChangesAsync();
        return WeatherDataConnectionFactory.Create(entity);
    }

    public async Task<IEnumerable<WeatherDataConnectionDtoModel>> GetManyById(int? cardId, int? weatherId)
    {
        var query = dbContext.WeatherDataConnections.AsNoTracking().AsQueryable();

        if (cardId != null)
            query = query.Where(w => w.CardId == cardId);
        if (weatherId != null)
            query = query.Where(w => w.WeatherId == weatherId);

        var entities = await query.ToListAsync();
        return entities.Select(WeatherDataConnectionFactory.Create);
    }

    public async Task<bool> Delete(int cardId)
    {
        var result = await dbContext.WeatherDataConnections.Where(w => w.CardId == cardId).ExecuteDeleteAsync();
        return result > 0;
    }
}
