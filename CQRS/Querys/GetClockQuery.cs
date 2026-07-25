using MediatR;

namespace Gridly.Querys;

public class GetClockQuery : IRequest<IResult>
{
    public string SearchTerm { get; set; }
}
