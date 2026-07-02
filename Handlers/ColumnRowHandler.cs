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
        var storedRowColumns = (await columnRowRepository.Get())?.ToList();
        
        await InsertNewRow();
        UpdateRowData();
        
        var flattCardList = storedRowColumns.SelectMany(x => x.Cards).ToList();
        await cardRepository.BatchEdit(flattCardList);
        
        return storedRowColumns.Any() ? Results.Ok(storedRowColumns.ToList()) : Results.NoContent();  
        
        async Task InsertNewRow()
        {    
            if (storedRowColumns!.Count < commands.Count)                                                       
            {                                                                                                  
                await columnRowRepository.Insert(commands.Last());                                             
                storedRowColumns = (await columnRowRepository.Get())?.ToList();                                
            }
            else if (storedRowColumns.Count > commands.Count)
            {
                var rowsMissing = storedRowColumns
                    .ExceptBy(commands.Select(c => c.Id), x => x.Id)
                    .Concat(commands.ExceptBy(storedRowColumns.Select(x => x.Id), c => c.Id));
                
                await columnRowRepository.BatchDelete(rowsMissing);
                storedRowColumns = (await columnRowRepository.Get())?.ToList();
            }
        }

        void UpdateRowData()
        { 
            foreach (var command in commands.ToList())                                                      
                foreach (var updated in storedRowColumns)                                                   
                {                                                                                           
                    if (command.RowPosition == updated.RowPosition)                                         
                    {                                                                                       
                        foreach (var card in command.Cards)                                                 
                        card.RowColumnId = updated.Id;                                                  
                                                                                                        
                        command.Id = updated.Id;                                                            
                        updated.Cards = command.Cards;                                                      
                    }                                                                                       
                }                                                                                           
        }
    }
}
