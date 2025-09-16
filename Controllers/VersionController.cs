using Gridly.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class VersionController(IMediator mediator) : ControllerBase
{ 
     public async Task<IResult> Get() => await mediator.Send(new GetVersionCommand());
     [EnableRateLimiting("TokenLimiter")]
     [HttpGet("latest")]
     public async Task<IResult> GetLatest() => await mediator.Send(new GetLatestVersionCommand());
}