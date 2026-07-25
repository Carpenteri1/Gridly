using Gridly.Querys;
using Gridly.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ClockController(IMediator mediator) : ControllerBase
{
    [HttpGet("get")]
    public async Task<IResult> Get([FromQuery] GetClockQuery query) => await mediator.Send(query);

    [EnableRateLimiting(RateLimiterPolicySettings.ClockPolicy)]
    [HttpGet("gettimeapidata")]
    public async Task<IResult> GetClockData([FromQuery] GetClockDataQuery query) => await mediator.Send(query);
}
