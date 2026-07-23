using Gridly.Dtos;
using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetWeatherCommand : IRequest<IResult>
{
    public string SearchTerm { get; set; }
}