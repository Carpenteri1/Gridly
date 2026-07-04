using Gridly.Command;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ColumnRowHandler(
    IColumnRowRepository columnRowRepository,
    ICardRepository cardRepository): 
    IRequestHandler<SaveColumnRowCommands, IResult>,
    IRequestHandler<GetAllRowColumnsCommands, IResult>,
    IRequestHandler<BatchEditRowColumnCommand, IResult>
{
    public async Task<IResult> Handle(SaveColumnRowCommands command, CancellationToken cancellationToken)
    {
        var rows = await columnRowRepository.Get();
        
        if (rows == null)
            await columnRowRepository.Insert(command);
        
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(GetAllRowColumnsCommands command, CancellationToken cancellationToken)
    {
        var storedRowColumns = (await columnRowRepository.Get())?.ToList();
        var cards = await cardRepository.Get();
        
            foreach (var row in storedRowColumns)
                foreach (var card in cards)
                    if (card.RowColumnId == row.Id)                          
                    {
                        if (row.Cards is null)
                            row.Cards = new List<CardModel>();             
                    
                        row.Cards.Add(card);                                
                    }
        
        return storedRowColumns.Any() ? Results.Ok(storedRowColumns.ToList()) : Results.NoContent();    
    }

    public async Task<IResult> Handle(BatchEditRowColumnCommand commands, CancellationToken cancellationToken)
    {
        var storedRowColumns = (await columnRowRepository.Get())?.ToList() ?? new List<ColumnRowModel>();
        var storedCards = (await cardRepository.Get())?.ToList() ?? new List<CardModel>();
        AttachCardsToRows(storedRowColumns, storedCards);

        await InsertNewRow();
        UpdateRowData();
        
        var flattCardList = commands.SelectMany(x => x.Cards).ToList();
        if (storedRowColumns.Count > commands.Count)
        {
            var rowsMissing = storedRowColumns
                .ExceptBy(commands.Select(c => c.Id), x => x.Id)
                .Concat(commands.ExceptBy(storedRowColumns.Select(x => x.Id), c => c.Id))
                .ToList();

            await cardRepository.BatchEdit(flattCardList);
            await columnRowRepository.BatchDelete(rowsMissing);
            storedRowColumns = (await columnRowRepository.Get())?.ToList() ?? new List<ColumnRowModel>();
        }
        else
        {
            await cardRepository.BatchEdit(flattCardList);
        }
        
        return storedRowColumns.Any() ? Results.Ok(storedRowColumns.ToList()) : Results.NoContent();  
        
        async Task InsertNewRow()
        {    
            if (storedRowColumns.Count < commands.Count)                                                       
            {                                                                                                  
                await columnRowRepository.Insert(commands.Last());                                             
                storedRowColumns = (await columnRowRepository.Get())?.ToList() ?? new List<ColumnRowModel>();                                
                AttachCardsToRows(storedRowColumns, storedCards);
            }
        }

        void UpdateRowData()
        { 
            foreach (var command in commands.ToList())                                                      
            {
                var updated = storedRowColumns.FirstOrDefault(row => row.Id == command.Id)
                    ?? storedRowColumns.FirstOrDefault(row => row.RowPosition == command.RowPosition);

                if (updated is null)
                    continue;

                foreach (var card in command.Cards)
                    card.RowColumnId = updated.Id;

                command.Id = updated.Id;
                updated.Cards = command.Cards;
            }
        }

        static void AttachCardsToRows(List<ColumnRowModel> rows, IEnumerable<CardModel> cards)
        {
            foreach (var row in rows)
            {
                row.Cards = cards
                    .Where(card => card.RowColumnId == row.Id)
                    .ToList();
            }
        }
    }
}
