using Gridly.Commands;
using Gridly.Configuration;
using Gridly.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ApiKeyController(IMediator mediator) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IResult> GetStatus([FromQuery] GetApiKeyStatusQuery query) => await mediator.Send(query);

    [EnableRateLimiting(RateLimiterPolicySettings.ApiKeyPolicy)]
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] SaveApiKeyCommand command) => await mediator.Send(command);
}
