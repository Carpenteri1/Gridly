using Gridly.Data;
using Gridly.Extension;
using Gridly.Models;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class CardRepository(GridlyDbContext dbContext) : ICardRepository
{
    public async Task<CardModel> Insert(CardModel card)
    {
        var entity = Factories.CardFactory.Create(card);
        dbContext.Cards.Add(entity);
        await dbContext.SaveChangesAsync();
        return Factories.CardFactory.Create(entity);
    }

    public async Task<bool> BatchEdit(IEnumerable<CardModel>? cards) => 
        await dbContext.SaveChangesAsync() > 0;

    public async Task<IEnumerable<CardModel>?> Get()
    {
        var entities = await dbContext.Cards
            .AsNoTracking()
            .WithSettingsAndIcons()
            .OrderBy(c => c.IndexPosition)
            .ToListAsync();

        return entities.Select(Factories.CardFactory.Create);
    }
    
    public async Task<bool> Delete(int id) => 
        await dbContext.Cards
            .WhereId(id)
            .ExecuteDeleteAsync() > 0;
}
