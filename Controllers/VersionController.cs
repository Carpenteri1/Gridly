using Gridly.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class VersionController(IMediator mediator) : ControllerBase
{ 
     [HttpGet("latest")]
     [EnableRateLimiting("TokenLimiter")]
     public async Task<IResult> GetLatest() => await mediator.Send(new GetLatestVersionCommand());
     [HttpGet("current")]
     public async Task<IResult> GetCurrent() => await mediator.Send(new GetCurrentVersionCommand());
     [HttpPost("save")]
     public async Task<IResult> SaveVersion([FromBody] SaveVersionCommand command) => await mediator.Send(command);
}