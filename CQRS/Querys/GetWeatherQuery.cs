using MediatR;

namespace Gridly.Querys;

public class GetWeatherQuery : IRequest<IResult>
{
    public required string Address { get; set; }
}