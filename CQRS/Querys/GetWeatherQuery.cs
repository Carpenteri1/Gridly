using MediatR;

namespace Gridly.Querys;

public class GetWeatherQuery : IRequest<IResult>
{
    public string Address { get; set; }
}