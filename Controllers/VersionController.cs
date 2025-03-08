using Gridly.Command;
using Gridly.Models;
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
     public async Task<VersionModel> GetLatest() => await mediator.Send(new GetAllLatestVersionCommand());
    [HttpGet("current")]
     public async Task<VersionModel> GetCurrent() => await mediator.Send(new GetAllCurrentVersionCommand());
}