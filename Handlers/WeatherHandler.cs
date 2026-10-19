using Gridly.Commands;
using Gridly.Constants;
using Gridly.Data;
using Gridly.EndPoints.Interfaces;
using Gridly.Enums;
using Gridly.Factories;
using Gridly.Querys;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Handlers;

public class WeatherHandler(
    IWeatherEndPoint weatherEndpoint,
    ILocalProvidersRepository localProvidersRepository,
    IProviderKeysProtectionService providerKeysProtectionService,
    GridlyDbContext dbContext) :
    IRequestHandler<GetWeatherQuery, IResult>,
    IRequestHandler<GetVisualCrossingDataQuery, IResult>,
    IRequestHandler<GetStoredWeatheDataQuery, IResult>,
    IRequestHandler<SaveWeatherCommand, IResult>
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromHours(8);

    public async Task<IResult> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        var weather = await dbContext.WeatherData
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Address == query.Address,cancellationToken);
        
        if(weather is null) return Results.NotFound();

        var isFresh = DateTime.UtcNow - weather.FetchedAt < CacheWindow;
        return isFresh ? Results.Ok(weather) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetVisualCrossingDataQuery query, CancellationToken cancellationToken)
    {
        var storedKeyEntity = await dbContext.ProviderKeys
            .FirstOrDefaultAsync(p => p.Provider == EndpointStrings.VisualCrossingProvider,cancellationToken);
        
        if (storedKeyEntity is null ||
            storedKeyEntity.Status == nameof(ProvidersKeyStatusEnum.Invalid) || 
            storedKeyEntity.Status is nameof(ProvidersKeyStatusEnum.Unknown) ||
            storedKeyEntity.EncryptedKey is null) return Results.Unauthorized();

        var rawKey = providerKeysProtectionService.Unprotect(storedKeyEntity.EncryptedKey);
        var (status, dto) = await weatherEndpoint.Get(query.Address, rawKey);
        
        if(dto is null) return Results.NotFound();
        
        switch (status)
        {
            case StatusCodes.Status401Unauthorized:
                await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Invalid));
                return Results.Unauthorized();
            case StatusCodes.Status403Forbidden:
                await localProvidersRepository.UpdateStatus(EndpointStrings.VisualCrossingProvider, nameof(ProvidersKeyStatusEnum.Invalid));
                return Results.Forbid();
            case < StatusCodes.Status200OK or >= StatusCodes.Status300MultipleChoices:
                return Results.BadRequest();
            case StatusCodes.Status200OK:
                //TODO move over to EF corex
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
        var weatherData = await dbContext.WeatherData
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return weatherData.Any() ? Results.Ok(weatherData.ToList()) : Results.NoContent();
    }

    public async Task<IResult> Handle(SaveWeatherCommand command, CancellationToken cancellationToken)
    {
        var weatherEntity = await dbContext.WeatherData
            .FirstOrDefaultAsync(w => w.Address == command.Weather.Address, cancellationToken);

        if (weatherEntity is null)
            weatherEntity = WeatherDataFactory.Create(command.Weather);
        
        var weatherDataConnectionEntity = await dbContext.WeatherDataConnections
            .FirstOrDefaultAsync(w => w.CardId == command.CardId,cancellationToken);

        if (weatherDataConnectionEntity is null || weatherDataConnectionEntity.WeatherId != weatherEntity.Id)
        { 
            weatherDataConnectionEntity = WeatherDataConnectionFactory.Create(command.CardId, weatherEntity.Id);
            await dbContext.WeatherDataConnections.AddAsync(weatherDataConnectionEntity,cancellationToken);
        }

        var success = await dbContext.SaveChangesAsync(cancellationToken);
        return success == 1 ? Results.Ok() : Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "ErrorSavingWeather");
    }
}
