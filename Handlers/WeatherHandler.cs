using Gridly.Command;
using Gridly.EndPoints;
using MediatR;

namespace Gridly.Handlers;

public class WeatherHandler(IWeatherEndPoint weatherEndpoint) : 
    IRequestHandler<GetWeatherCommand, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    public async Task<IResult> Handle(GetWeatherCommand command, CancellationToken cancellationToken)
    {
        //TODO Cashing and store in database, look up if location is stored already, if not, then look at the api.   
        var (success, weather) = await weatherEndpoint.Get(command.SearchTerm);
        if(!success) return Results.NotFound();
        return Results.Ok(weather);
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        // TODO implement
        return Results.Ok();
    }
}
