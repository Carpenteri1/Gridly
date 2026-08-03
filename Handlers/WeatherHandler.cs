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
    ILocalProvidersRepository localProvidersRepository,
    IProviderKeysProtectionService providerKeysProtectionService) :
    IRequestHandler<GetWeatherQuery, IResult>,
    IRequestHandler<GetVisualCrossingDataQuery, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromHours(8);

    public async Task<IResult> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        var (weather, fetchedAt) = await weatherRepository.Get(query.SearchTerm);
        if(weather is null) return Results.NotFound();
        
        var isFresh = fetchedAt is not null && DateTime.UtcNow - fetchedAt.Value < CacheWindow;
        if(!isFresh)
            await weatherRepository.Delete(weather.CardId);
        
        return isFresh ? Results.Ok(weather) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetVisualCrossingDataQuery query, CancellationToken cancellationToken)
    {
        if(query.SearchTerm is null) return Results.BadRequest();
        
        var storedKey = await localProvidersRepository.Get(EndpointStrings.VisualCrossingProvider);
        
        if(storedKey is null) return Results.Unauthorized();
        if(storedKey.Status == nameof(ProvidersKeyStatusEnum.Invalid) || 
           storedKey.Status is nameof(ProvidersKeyStatusEnum.Unknown)) return Results.Unauthorized();
        
        var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey);
        var (status, weather) = await weatherEndpoint.Get(query.SearchTerm, rawKey);
        
        switch (status)
        {
            case StatusCodes.Status401Unauthorized:
                await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Invalid));
                return Results.Unauthorized();
            case StatusCodes.Status403Forbidden:
                await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Valid));
                return Results.Forbid();
            case < StatusCodes.Status200OK or >= StatusCodes.Status300MultipleChoices:
                return Results.BadRequest();
            case StatusCodes.Status200OK:
                await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Valid));
                return Results.Ok(weather);
            default:
                return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, detail: "ProviderUnavailable");
        }
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        command.Weather.FetchedAt = DateTime.UtcNow;
        var success = await weatherRepository.Upsert(command.Weather);
        return success ? Results.Ok() : Results.BadRequest();
    }
}
