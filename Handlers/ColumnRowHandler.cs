using Gridly.Commands;
using Gridly.Data;
using Gridly.Extension;
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
        
        return storedRowColumns.Any() ? Results.Ok(storedRowColumns) : Results.NoContent();    
    }

    public async Task<IResult> Handle(BatchSaveColumnRowCommands commands, CancellationToken cancellationToken)
    {
        var existingRows = await dbContext.RowColumns
            .GetRowsAndConnectedCards()
            .ToListAsync(cancellationToken);

        var rowsToDelete = commands.Where(command => command.Cards.Count == 0).ToList();
        var rowsToSave = commands.Where(command => command.Cards.Count > 0).ToList();

        //await dbContext.SaveChangesAsync(cancellationToken);

        return rowsToSave.Any() ? Results.Ok(rowsToSave) : Results.NoContent();
    }
}
