using Gridly.Querys;
using Gridly.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class VersionController(IMediator mediator) : ControllerBase
{ 
     [HttpGet("get")]
     public async Task<IResult> Get() => await mediator.Send(new GetVersionQuery());
     [EnableRateLimiting(RateLimiterPolicySettings.VersionPolicy)]
     [HttpGet("latest")]
     public async Task<IResult> GetLatest() => await mediator.Send(new GetLatestVersionQuery());
}
