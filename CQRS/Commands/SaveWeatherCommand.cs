using Gridly.Dtos;
using MediatR;

namespace Gridly.Commands;

public class SaveWeatherCommand : IRequest<IResult>
{
    public required WeatherDataModel Weather { get; set; }
    public int CardId { get; set; }
}