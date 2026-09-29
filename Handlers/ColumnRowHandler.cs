using Gridly.Commands;
using Gridly.Data;
using Gridly.Extension;
using Gridly.Factories;
using Gridly.Querys;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Handlers;
public class ColumnRowHandler(
    GridlyDbContext dbContext):
    IRequestHandler<GetAllRowColumnsQuery, IResult>,
    IRequestHandler<BatchSaveColumnRowCommands, IResult>
{
    public async Task<IResult> Handle(GetAllRowColumnsQuery query, CancellationToken cancellationToken)
    {
        var storedRowColumns = await dbContext.RowColumns
            .AsNoTracking()
            .GetRowsAndConnectedCards()
            .OrderBy(row => row.RowPosition)
            .ToListAsync(cancellationToken);

        var rows = ColumnRowFactory.CreateMany(storedRowColumns).ToList();

        return rows.Any() ? Results.Ok(rows) : Results.NoContent();
    }

    public async Task<IResult> Handle(BatchSaveColumnRowCommands commands, CancellationToken cancellationToken)
    {
        var existingRows = await dbContext.RowColumns
            .ToDictionaryAsync(row => row.Id, cancellationToken);
        var existingCards = await dbContext.Cards
            .ToDictionaryAsync(card => card.Id, cancellationToken);

        var keptRowIds = new HashSet<int>();
        var keptCardIds = new HashSet<int>();

        foreach (var command in commands.Where(command => command.Cards.Any()))
        {
            if (!existingRows.TryGetValue(command.Id, out var rowEntity))
            {
                rowEntity = ColumnRowFactory.Create(command);
                rowEntity.Id = 0;
                dbContext.RowColumns.Add(rowEntity);
            }

            keptRowIds.Add(rowEntity.Id);
            rowEntity.RowPosition = command.RowPosition;
            rowEntity.RowWidth = command.RowWidth;
            rowEntity.Cards ??= [];

            foreach (var card in command.Cards)
            {
                if (!existingCards.TryGetValue(card.Id, out var cardEntity))
                {
                    rowEntity.Cards.Add(CardFactory.Create(card));
                    continue;
                }

                keptCardIds.Add(cardEntity.Id);

                cardEntity.IndexPosition = card.IndexPosition;
                
                if (!rowEntity.Cards.Contains(cardEntity))
                    rowEntity.Cards.Add(cardEntity);
            }
        }
        
        dbContext.ChangeTracker.DetectChanges();

        var cardIdsToDelete = existingCards.Keys
            .Where(cardId => !keptCardIds.Contains(cardId))
            .ToList();

        if (cardIdsToDelete.Count > 0)
        {
            var iconLinksToDelete = await dbContext.IconsConnected
                .Where(link => link.CardId != null && cardIdsToDelete.Contains(link.CardId.Value))
                .ToListAsync(cancellationToken);
            var weatherLinksToDelete = await dbContext.WeatherDataConnections
                .Where(link => cardIdsToDelete.Contains(link.CardId))
                .ToListAsync(cancellationToken);

            dbContext.IconsConnected.RemoveRange(iconLinksToDelete);
            dbContext.WeatherDataConnections.RemoveRange(weatherLinksToDelete);
            dbContext.Cards.RemoveRange(cardIdsToDelete.Select(cardId => existingCards[cardId]));
        }

        dbContext.RowColumns.RemoveRange(existingRows.Values
            .Where(row => !keptRowIds.Contains(row.Id)));

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}
