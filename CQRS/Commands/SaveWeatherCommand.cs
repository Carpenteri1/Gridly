using Gridly.Models;
using MediatR;

namespace Gridly.Commands;

public class SaveWeatherCommand : IRequest<IResult>
{
    public string Location { get; set; }
    public WeatherModel Weather { get; set; }
}