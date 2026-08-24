using Gridly.Data;
using Gridly.Dtos;
using Gridly.Extension;
using Gridly.Factories;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class IconConnectedRepository(GridlyDbContext dbContext) : IIconConnectedRepository
{
    public async Task<IconConnectedDtoModel> Insert(IconConnectedDtoModel dto)
    {
        var entity  = IconConnectedFactory.Create(dto);
        dbContext.IconsConnected.Add(entity);
        await dbContext.SaveChangesAsync();
        return IconConnectedFactory.Create(entity);
    }

    public async Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? cardId, int? iconId)
    {
        var entities = await dbContext.IconsConnected
            .WhereId(cardId, iconId)
            .ToListAsync();
        
        return entities.Select(IconConnectedFactory.Create);
    }

    public async Task<bool> Delete(int cardId) => 
        await dbContext.IconsConnected.Where(ic => ic.CardId == cardId).ExecuteDeleteAsync() > 0;
}
