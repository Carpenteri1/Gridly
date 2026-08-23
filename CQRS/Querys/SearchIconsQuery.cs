using MediatR;

namespace Gridly.Querys;

public class SearchIconsQuery : IRequest<IResult> 
{
    public required string SearchTerm { get; set; }
}