using Gridly.Command;
using MediatR;

namespace Gridly.Handlers;

public class WeatherHandler() :
    IRequestHandler<GetWeatherCommand, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    public async Task<IResult> Handle(GetWeatherCommand command, CancellationToken cancellationToken)
    {
        // TODO implement
        return Results.Ok();
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        // TODO implement
        return Results.Ok();
    }
}
