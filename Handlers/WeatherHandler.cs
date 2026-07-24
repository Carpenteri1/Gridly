using Gridly.Querys;
using Gridly.Commands;
using Gridly.EndPoints;
using MediatR;

namespace Gridly.Handlers;

public class WeatherHandler(IWeatherEndPoint weatherEndpoint) : 
    IRequestHandler<GetWeatherQuery, IResult>,
    IRequestHandler<GetVisualCrossingDataQuery, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    public async Task<IResult> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        //TODO Cashing and store in database, look up if location is stored already, if not return not found.   
        return Results.Ok();
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        // TODO implement
        return Results.Ok();
    }

    public async Task<IResult> Handle(GetVisualCrossingDataQuery query, CancellationToken cancellationToken)
    {
        //TODO if not found in database, get from third party api and store in database and cache.   
        var (success, weather) = await weatherEndpoint.Get(query.SearchTerm);
        if(!success) return Results.NotFound();
        
        return Results.Ok(weather);
    }
}
