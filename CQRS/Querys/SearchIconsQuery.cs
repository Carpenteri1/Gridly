using MediatR;

namespace Gridly.Querys;

public class SearchIconsQuery : IRequest<IResult> 
{
    public string Value { get; set; }
}