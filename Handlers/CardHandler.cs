using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class CardHandler(
    ICardRepository cardRepository) :
    IRequestHandler<GetAllCardQuery, IResult>
{
    public async Task<IResult> Handle(GetAllCardQuery query, CancellationToken cancellationToken)
    {
        var cards = await cardRepository.Get() ?? Enumerable.Empty<CardModel>();
        return Results.Ok(cards.OrderBy(c => c.RowColumnId).ThenBy(x => x.IndexPosition));
    }
}
