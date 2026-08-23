using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Factories;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class IconConnectedRepository(GridlyDbContext dbContext) : IIconConnectedRepository
{
    public async Task<IconConnectedDtoModel> Insert(IconConnectedDtoModel model)
    {
        var entity = new IconsConnectedEntity
        {
            CardId = model.CardId,
            IconId = model.IconId,
        };
        dbContext.IconsConnected.Add(entity);
        await dbContext.SaveChangesAsync();
        return IconConnectedFactory.Create(entity);
    }

    public async Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? cardId, int? iconId)
    {
        var query = dbContext.IconsConnected.AsQueryable();
        if (cardId != null)
            query = query.Where(ic => ic.CardId == cardId);
        if (iconId != null)
            query = query.Where(ic => ic.IconId == iconId);

        var entities = await query.ToListAsync();
        return entities.Select(IconConnectedFactory.Create);
    }

    public async Task<bool> Delete(int cardId)
    {
        var result = await dbContext.IconsConnected.Where(ic => ic.CardId == cardId).ExecuteDeleteAsync();
        return result > 0;
    }
}
