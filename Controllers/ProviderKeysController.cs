using Gridly.Commands;
using Gridly.Configuration;
using Gridly.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProviderKeysController(IMediator mediator) : ControllerBase
{
    [HttpGet("remote/provider/status")]
    [EnableRateLimiting(RateLimiterPolicySettings.RemoteProviderKeyPolicy)]
    public async Task<IResult> GetStatus([FromQuery] GetRemoteProviderKeyStatusQuery query) => await mediator.Send(query);
    [HttpGet("local/provider/status")]
    public async Task<IResult> GetStatus([FromQuery] GetLocalProviderKeyStatusQuery query) => await mediator.Send(query);
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] SaveProviderKeyCommand command) => await mediator.Send(command);
}
