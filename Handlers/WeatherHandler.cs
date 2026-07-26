using Gridly.Commands;
using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.Enums;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class WeatherHandler(
    IWeatherEndPoint weatherEndpoint,
    IWeatherRepository weatherRepository,
    IProvidersRepository providersRepository,
    IProviderKeysProtectionService providerKeysProtectionService) :
    IRequestHandler<GetWeatherQuery, IResult>,
    IRequestHandler<GetVisualCrossingDataQuery, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromHours(8);

    public async Task<IResult> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        var (weather, fetchedAt) = await weatherRepository.Get(query.SearchTerm);
        var isFresh = weather is not null && fetchedAt is not null && DateTime.UtcNow - fetchedAt.Value < CacheWindow;

        return isFresh ? Results.Ok(weather) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetVisualCrossingDataQuery query, CancellationToken cancellationToken)
    {
        if(query.Location is null) return Results.BadRequest();
        
        var storedKey = await providersRepository.Get(EndpointStrings.VisualCrossingProvider);
        
        if(storedKey is null) return Results.Unauthorized();
        if(storedKey.Status == nameof(ProvidersKeyStatusEnum.Invalid) || 
           storedKey.Status is nameof(ProvidersKeyStatusEnum.Unknown)) return Results.Unauthorized();
        
        var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey);
        var (status, weather) = await weatherEndpoint.Get(query.Location, storedKey.Provider, rawKey);
        
        switch (status)
        {
            case StatusCodes.Status401Unauthorized:
                await providersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Invalid));
                return Results.Unauthorized();
            case StatusCodes.Status403Forbidden:
                await providersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Valid));
                return Results.Forbid();
            case < StatusCodes.Status200OK or >= StatusCodes.Status300MultipleChoices:
                return Results.BadRequest();
            case StatusCodes.Status200OK: 
                await providersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Valid));
                return Results.Ok(weather);
            default:
                var (staleWeather, _) = await weatherRepository.Get(query.Location);
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
