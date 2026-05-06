using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class CardRepository(IDbConnection connection) : ICardRepository
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
                Width = c.Settings?.Width ?? 250,
                Height = c.Settings?.Height ?? 250
            })
            .ToList();

        var result = await connection.ExecuteAsync(QueryStrings.UpdateBatchCardQuery, parameters);
        return result > 0;
    }

    public async Task<IEnumerable<CardModel>?> Get()
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectCardQuery);
        
        builder.LeftJoin(QueryStrings.JoinSettingsQuery);
        builder.LeftJoin(QueryStrings.JoinIconsConnectedDataQuery);
        builder.LeftJoin(QueryStrings.JoinIconDataQuery);
        builder.OrderBy(QueryStrings.IndexPositionWithAlias);
        var Dtos = 
            await _dbCommandRunner.SelectMany<CardDtoModel>(template.RawSql, template.Parameters);
        return Factories.CardFactory.CreateMany(Dtos);
    }
    
    public async Task<CardModel?> GetById(int cardId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectCardQuery);
        
        builder.LeftJoin(QueryStrings.JoinSettingsQuery);
        builder.LeftJoin(QueryStrings.JoinIconsConnectedDataQuery);
        builder.LeftJoin(QueryStrings.JoinIconDataQuery);
        builder.Where(QueryStrings.WhereCardIdEqualsCardIdWithAlias, new {cardId});
        var dto = await _dbCommandRunner.Select<CardDtoModel>(template.RawSql,template.Parameters);
        return Factories.CardFactory.Create(dto);
    }

    public async Task<bool> Delete(int id)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.DeleteFromCardQuery);
        builder.Where(QueryStrings.WhereIdEqualsId, new { Id = id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
}
