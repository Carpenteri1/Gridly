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
    [EnableRateLimiting("fixed")]
     public async Task<VersionModel> Get() => await mediator.Send(new GetAllVersionCommand());
}