using MediatR;

namespace Gridly.Querys;

public class GetVisualCrossingDataQuery : IRequest<IResult>
{
    public string Location { get; set; }
}