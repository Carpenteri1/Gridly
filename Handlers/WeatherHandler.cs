using Gridly.Commands;
using Gridly.Constants;
using Gridly.EndPoints;
using Gridly.Enums;
using Gridly.Factories;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class WeatherHandler(
    IWeatherEndPoint weatherEndpoint,
    IWeatherRepository weatherRepository,
    IWeatherDataConnectionRepository weatherDataConnectionRepository,
    ILocalProvidersRepository localProvidersRepository,
    IProviderKeysProtectionService providerKeysProtectionService) :
    IRequestHandler<GetWeatherQuery, IResult>,
    IRequestHandler<GetVisualCrossingDataQuery, IResult>,
    IRequestHandler<GetStoredWeatheDataQuery, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromHours(8);

    public async Task<IResult> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        var weather = await weatherRepository.Get(query.Address);
        if(weather is null) return Results.NotFound();
        
        var isFresh = DateTime.UtcNow - weather.FetchedAt < CacheWindow;
        return isFresh ? Results.Ok(weather) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetVisualCrossingDataQuery query, CancellationToken cancellationToken)
    {
        var storedKey = await localProvidersRepository.Get(EndpointStrings.VisualCrossingProvider);
        
        if(storedKey is null) return Results.Unauthorized();
        if(storedKey.Status == nameof(ProvidersKeyStatusEnum.Invalid) || 
           storedKey.Status is nameof(ProvidersKeyStatusEnum.Unknown)) return Results.Unauthorized();
        
        var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey);
        var (status, dto) = await weatherEndpoint.Get(query.Address, rawKey);
        
        if(dto is null) return Results.NotFound();
        
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
                var weather = WeatherDataFactory.Create(dto);
                weather.FetchedAt = DateTime.UtcNow;
                return Results.Ok(weather);
            default:
                return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, detail: "ProviderUnavailable");
        }
    }
    
    public async Task<IResult> Handle(GetStoredWeatheDataQuery request, CancellationToken cancellationToken)
    {
        var storedWeatherData = await weatherRepository.GetStoredWeatherData();
        return storedWeatherData is not null ? Results.Ok(storedWeatherData) : Results.NoContent();
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        var previousConnection = (await weatherDataConnectionRepository.GetManyById(command.CardId, null)).FirstOrDefault();

        var weather = await weatherRepository.Upsert(command.Weather);
        if (weather is null) return Results.BadRequest();

        await weatherDataConnectionRepository.Upsert(WeatherDataConnectionFactory.Create(command.CardId, weather.Id));

        if (previousConnection?.WeatherId is not null && previousConnection.WeatherId != weather.Id)
            await weatherRepository.DeleteIfOrphaned(previousConnection.WeatherId.Value);

        return Results.Ok();
    }
}
