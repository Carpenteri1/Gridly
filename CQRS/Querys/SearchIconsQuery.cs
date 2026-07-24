using MediatR;

namespace Gridly.Command;

public class SearchIconsQuery : IRequest<IResult> 
{
    public string Value { get; set; }
}