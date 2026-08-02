using Gridly.Models;
using MediatR;

namespace Gridly.Commands;

public class SaveWeatherCommand : IRequest<IResult>
{
    public WeatherModel Weather { get; set; }
}