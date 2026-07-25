using MediatR;

namespace Gridly.Querys;

public class GetClockDataQuery : IRequest<IResult>
{
    public string SearchTerm { get; set; }
}
