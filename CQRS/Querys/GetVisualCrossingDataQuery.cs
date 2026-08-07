using MediatR;

namespace Gridly.Querys;

public class GetVisualCrossingDataQuery : IRequest<IResult>
{
    public string Address { get; set; }
}