using MediatR;

namespace Gridly.Querys;

public class GetWeatherQuery : IRequest<IResult>
{
    public string SearchTerm { get; set; }
}