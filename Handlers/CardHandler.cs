using Gridly.Command;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class CardHandler(
    ICardRepository cardRepository) :
    IRequestHandler<GetAllCardCommand, IResult>
{
    public async Task<IResult> Handle(GetAllCardCommand command, CancellationToken cancellationToken)
    {
        var cards = await cardRepository.Get();
        return Results.Ok(cards.OrderBy(c => c.RowColumnId).ThenBy(x => x.IndexPosition));
    }
}
