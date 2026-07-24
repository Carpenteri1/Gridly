using MediatR;

namespace Gridly.Querys;

public class SearchIconsQuery : IRequest<IResult> 
{
    public string SearchTerm { get; set; }
}