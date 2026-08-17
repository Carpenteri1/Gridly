using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class CardRepository(IDbConnection connection, IGridlyDbContext dbContext) : ICardRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<CardModel> Insert(CardModel Card)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToCardQuery, Card);
    }
    
    
    public async Task<bool> Edit(CardModel Card)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateCardQuery,Card);
        builder.Where(QueryStrings.WhereIdEqualsId,  new { Id = Card.Id });
        return await _dbCommandRunner.Execute(template.RawSql,template.Parameters);
    }

    public async Task<bool> BatchEdit(IEnumerable<CardModel>? cards)
    {
        if (cards is null)
            return false;

        var parameters = cards
            .Select(c => new 
            { 
                c.Id,
                c.IndexPosition,
                c.RowColumnId,
                Width = c.Settings?.Width ?? 250,
                Height = c.Settings?.Height ?? 250,
                TitleHidden = c.Settings?.TitleHidden ?? false,
                ImageHidden = c.Settings?.ImageHidden ?? false
            })
            .ToList();

        var result = await connection.ExecuteAsync(QueryStrings.UpdateBatchCardQuery, parameters);
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

        // IGridlyDbContext exposes IQueryable<T> (not DbSet<T>) so it can be swapped for a plain
        // in-memory fake in tests; that fake's LINQ-to-Objects provider doesn't implement
        // IAsyncEnumerable, so EF's ToListAsync() would throw against it. Materializing
        // synchronously works against both the fake and the real EF-backed query.
        var dtos = query.ToList();
        return Task.FromResult<IEnumerable<CardModel>?>(Factories.CardFactory.CreateMany(dtos));
    }

    public async Task<bool> Delete(int id)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.DeleteFromCardQuery);
        builder.Where(QueryStrings.WhereIdEqualsId, new { Id = id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
}
