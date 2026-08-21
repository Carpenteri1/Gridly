using Gridly.Commands;
using Gridly.Configuration;
using Gridly.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Services;

public static class ApiServices
{ 
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        MapIconEndpoints(app);
        MapProviderEndpoints(app);
        MapRowEndpoints(app);
        MapVersionEndpoints(app);
        MapWeatherEndpoints(app);
        MapWidgetEndpoints(app);
    }
    
    private static void MapIconEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/icon/get", async (IMediator mediator) =>
            await mediator.Send(new GetIconQuery()));
        app.MapGet("/api/icon/search", async ([AsParameters] SearchIconsQuery query, IMediator mediator) =>
        await mediator.Send(query));
    }
    
    private static void MapProviderEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/keyProvider/remote/status",
            async ([AsParameters] GetRemoteProviderKeyStatusQuery query, IMediator mediator) =>
            await mediator.Send(query)).RequireRateLimiting(RateLimiterPolicySettings.RemoteKeyProviderPolicy);

        app.MapGet("/api/keyProvider/local/status",
            async ([AsParameters] GetLocalProviderKeyStatusQuery query, IMediator mediator) =>
            await mediator.Send(query));
        
        app.MapPost("/api/keyProvider/save", 
            async ([FromBody] SaveProviderKeyCommand command, IMediator mediator) =>
            await mediator.Send(command));
    }
    
    private static void MapRowEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/row/get", async (IMediator mediator) =>
            await mediator.Send(new GetAllRowColumnsQuery()));

        app.MapPost("/api/row/batchSave",
            async ([FromBody] BatchSaveColumnRowCommands commands, IMediator mediator) =>
            await mediator.Send(commands));
    }
    
    private static void MapVersionEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/version/get", async (IMediator mediator) =>
            await mediator.Send(new GetVersionQuery()));
        
        app.MapGet("/api/version/latest", async (IMediator mediator) => 
                await mediator.Send(new GetLatestVersionQuery()))
            .RequireRateLimiting(RateLimiterPolicySettings.VersionProviderPolicy);
    }
    
    private static void MapWeatherEndpoints(IEndpointRouteBuilder app)
    {
        
        app.MapGet("/api/weather/get",
            async ([AsParameters] GetWeatherQuery query ,IMediator mediator) =>
            await mediator.Send(query));

        app.MapGet("/api/weather/getvisualcrossingdata",
                async ([AsParameters] GetVisualCrossingDataQuery query, IMediator mediator) =>
                await mediator.Send(query))
            .RequireRateLimiting(RateLimiterPolicySettings.WeatherProviderPolicy);

        app.MapGet("/api/weather/getstoredweatherdata",
            async ([AsParameters] GetStoredWeatheDataQuery query, IMediator mediator) =>
            await mediator.Send(query));

        app.MapPost("/api/weather/save", 
            async ([FromBody] SaveWeatherCommand command, IMediator mediator) => 
            await mediator.Send(command));
    }

    private static void MapWidgetEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/widget/get", 
            async (IMediator mediator) => 
                await mediator.Send(new GetWidgetQuery()));
    }
}