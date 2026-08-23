using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class CardRepository(GridlyDbContext dbContext) : ICardRepository
{
    public async Task<CardModel> Insert(CardModel Card)
    {
        var entity = new CardEntity
        {
            RowColumnId = Card.RowColumnId!.Value,
            IndexPosition = Card.IndexPosition!.Value,
            Name = Card.Name,
            Url = Card.Url,
            IconUrl = Card.IconUrl,
            Type = Card.Type,
        };
        dbContext.Cards.Add(entity);
        await dbContext.SaveChangesAsync();
        return Factories.CardFactory.Create(entity);
    }

    public async Task<bool> BatchEdit(IEnumerable<CardModel>? cards)
    {
        if (cards is null)
            return false;

        var result = await dbContext.SaveChangesAsync();
        return result > 0;
    }

    public Task<IEnumerable<CardModel>?> Get()
    {
        var query =
            from co in dbContext.Cards.AsNoTracking()
            join cs in dbContext.Settings on co.Id equals cs.CardId
            join ic in dbContext.IconsConnected on (int?)co.Id equals ic.CardId
            join i in dbContext.Icons on ic.IconId equals (int?)i.Id
            orderby co.IndexPosition
            select new CardDtoModel
            {
                CardId = co.Id,
                IndexPosition = co.IndexPosition,
                RowColumnId = co.RowColumnId,
                CardName = co.Name!,
                Url = co.Url!,
                IconUrl = co.IconUrl!,
                CardType = co.Type!,
                SettingsId = cs.Id,
                Width = cs.Width,
                Height = cs.Height,
                TitleHidden = cs.TitleHidden ?? false,
                ImageHidden = cs.ImageHidden ?? false,
                IconId = i.Id,
                IconName = i.Name!,
                Type = i.Type!,
                Base64Data = i.Base64Data!,
                MaterialIcon = i.MaterialIcon!,
            };
        var dtos = query.ToList();
        return Task.FromResult<IEnumerable<CardModel>?>(Factories.CardFactory.CreateMany(dtos));
    }

    public async Task<bool> Delete(int id)
    {
        var rowsAffected = await dbContext.Cards
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
        return rowsAffected > 0;
    }
}
