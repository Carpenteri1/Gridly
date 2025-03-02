using Gridly.Handlers;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class VersionController(IVersionHandler<VersionModel> version) : ControllerBase
{
    //[HttpGet("latest")]
    //[EnableRateLimiting("fixed")]
   // public async Task<VersionModel> Get() => await versionCommand.Handler();
}