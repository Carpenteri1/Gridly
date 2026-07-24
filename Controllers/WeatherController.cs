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
    //[EnableRateLimiting(RateLimiterPolicySettings.WeatherPolicy)]
    [HttpGet("get")]
    public async Task<IResult> Get([FromQuery] GetWeatherQuery command) => await mediator.Send(command);
    
    [EnableRateLimiting(RateLimiterPolicySettings.WeatherPolicy)]
    [HttpGet("getvisualcrossingdata")]
    public async Task<IResult> GetWeather([FromQuery] GetVisualCrossingDataQuery command) => await mediator.Send(command);

    [HttpPost("save")]
    public async Task<IResult> SaveSettings([FromBody] SaveWeatherCommand command) =>
        await mediator.Send(command);
}
