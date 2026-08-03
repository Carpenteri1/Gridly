using Gridly.Dtos;
using MediatR;

namespace Gridly.Commands;

public class SaveWeatherCommand : IRequest<IResult>
{
    public WeatherDataDtoModel Weather { get; set; }
}