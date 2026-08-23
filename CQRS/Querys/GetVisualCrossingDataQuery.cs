using MediatR;

namespace Gridly.Querys;

public class GetVisualCrossingDataQuery : IRequest<IResult>
{
    public required string Address { get; set; }
}