using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class SearchIconsCommand : IconModel, IRequest<IResult> 
{
    public string Value { get; set; }
}