using MediatR;

namespace Gridly.Querys;

public class GetVisualCrossingDataQuery : IRequest<IResult>
{
    public string SearchTerm { get; set; }
}