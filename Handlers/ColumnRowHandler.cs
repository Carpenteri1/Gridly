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
            .GetRowsAndConnectedCards()
            .ToListAsync(cancellationToken);
        
        if (existingRows.Count == 0)
        {
            var rowEntities = commands.Select(command =>
            {
                var rowEntity = ColumnRowFactory.Create(command);
                rowEntity.Cards = CardFactory.CreateMany(command.Cards).ToList();
                return rowEntity;
            }).ToList();

            await dbContext.RowColumns.AddRangeAsync(rowEntities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok();
        }
        
        //TODO seemes to break view below here
        var rowsToDelete = commands.Where(command => command.Cards.Count() == 0).ToList();
        var rowsToUpdate = commands.Where(command => command.Cards.Count() > 0).ToList();
        
        if (rowsToUpdate.Any())
        {
            foreach (var existingRow in existingRows)
            {
                foreach (var row in rowsToUpdate)
                {
                    if (existingRow.Id != row.Id) continue;
                    
                    //Move card to new row
                    if (existingRow.Cards != null && 
                        existingRow.Cards.Count != row.Cards.Count())
                    {
                        foreach (var existingCard in existingRow.Cards)
                        {
                            foreach (var card in row.Cards)
                            {
                                if (card.Id != existingCard.Id) continue;
                                existingCard.IndexPosition = card.IndexPosition;
                                existingCard.RowColumnId = card.RowColumnId;
                            }   
                        }
                        
                    }
                    existingRow.Cards = Factories.CardFactory.CreateMany(row.Cards).ToList();
                }
            }   
        }

        if (rowsToDelete.Any())
        {
            foreach (var existingRow in existingRows)
            {
                foreach (var row in rowsToDelete)
                {
                    if (existingRow.Id != row.Id) continue;
                    dbContext.RowColumns.Remove(existingRow);
                }   
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return rowsToUpdate.Any() ? Results.Ok(rowsToUpdate) : Results.NoContent();
    }
}
