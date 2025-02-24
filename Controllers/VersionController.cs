using Gridly.Handlers;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class VersionController(IVersionHandler handler) : ControllerBase
{
    [HttpGet("latest")]
    [EnableRateLimiting("fixed")]
    public async Task<VersionModel> Get() => await handler.GetVersionAsync();
}