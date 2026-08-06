using Gridly.Querys;
using Gridly.Commands;
using Gridly.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class WeatherController(IMediator mediator) : ControllerBase
{ 
    [HttpGet("get")]
    public async Task<IResult> Get([FromQuery] GetWeatherQuery query) => await mediator.Send(query);
    
    [EnableRateLimiting(RateLimiterPolicySettings.WeatherProviderPolicy)]
    [HttpGet("getvisualcrossingdata")]
    public async Task<IResult> GetWeather([FromQuery] GetVisualCrossingDataQuery query) => await mediator.Send(query);
    
    [HttpGet("getstoredweatherdata")]
    public async Task<IResult> GetWeather([FromQuery] GetStoredWeatheDataQuery query) => await mediator.Send(query);

    [HttpPost("save")]
    public async Task<IResult> SaveSettings([FromBody] SaveWeatherCommand command) =>
        await mediator.Send(command);
}
