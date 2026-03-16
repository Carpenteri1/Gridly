using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class SearchIconsCommand : IRequest<IResult> 
{
    public string Value { get; set; }
}