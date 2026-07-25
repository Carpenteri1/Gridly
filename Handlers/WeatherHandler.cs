using Gridly.Commands;
using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using MediatR;

namespace Gridly.Handlers;

public class WeatherHandler(
    IWeatherEndPoint weatherEndpoint,
    IWeatherRepository weatherRepository,
    IApiKeyRepository apiKeyRepository) :
    IRequestHandler<GetWeatherQuery, IResult>,
    IRequestHandler<GetVisualCrossingDataQuery, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromMinutes(30);

    public async Task<IResult> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        var (weather, fetchedAt) = await weatherRepository.Get(query.SearchTerm);
        var isFresh = weather is not null && fetchedAt is not null && DateTime.UtcNow - fetchedAt.Value < CacheWindow;

        return isFresh ? Results.Ok(weather) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetVisualCrossingDataQuery query, CancellationToken cancellationToken)
    {
        var (status, weather) = await weatherEndpoint.Get(query.SearchTerm);

        switch (status)
        {
            case WeatherFetchStatus.Success:
                await weatherRepository.Upsert(query.SearchTerm, weather!);
                return Results.Ok(weather);

            case WeatherFetchStatus.InvalidApiKey:
                await apiKeyRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, ApiKeyStatus.Invalid);
                return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "ApiKeyInvalid");

            case WeatherFetchStatus.NoApiKey:
                return Results.Problem(statusCode: StatusCodes.Status412PreconditionFailed, detail: "ApiKeyMissing");

            case WeatherFetchStatus.ProviderUnavailable:
            default:
                var (staleWeather, _) = await weatherRepository.Get(query.SearchTerm);
                return staleWeather is not null
                    ? Results.Ok(staleWeather)
                    : Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, detail: "ProviderUnavailable");
        }
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        var success = await weatherRepository.Upsert(command.Location, command.Weather);
        return success ? Results.Ok() : Results.BadRequest();
    }
}
