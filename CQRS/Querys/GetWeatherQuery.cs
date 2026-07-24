using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetWeatherQuery : IRequest<IResult>
{
    public string SearchTerm { get; set; }
}