using Gridly.Command;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ColumnRowHandler(
    IColumnRowRepository columnRowRepository,
    ICardRepository cardRepository): 
    IRequestHandler<SaveColumnRowCommands, IResult>,
    IRequestHandler<GetAllRowColumnsCommands, IResult>
{
    public async Task<IResult> Handle(SaveColumnRowCommands command, CancellationToken cancellationToken)
    {
        var rows = await columnRowRepository.Get();
        
        if (rows == null)
            await columnRowRepository.Insert(command);
        
        /*int totalCardWith = 0;
        foreach (var row in rows)
        {
            if (row.RowPosition == command.RowPosition)
            {
                
            }
        }*/
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(GetAllRowColumnsCommands command, CancellationToken cancellationToken)
    {
        var rowColummns = (await columnRowRepository.Get())?.ToList();
        var cards = await cardRepository.Get();
        foreach (var row in rowColummns)
            foreach (var card in cards)
                if (card.RowColumnId == row.Id)                          
                {
                    if (row.Cards is null)
                        row.Cards = new List<CardModel>();             
                    
                    row.Cards.Add(card);                                
                }
        
        return rowColummns.Any() ? Results.Ok(rowColummns.ToList()) : Results.NoContent();    
    }
}
