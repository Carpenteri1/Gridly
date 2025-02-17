using Gridly.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet("latest")]
    public async Task<string> GetLatestRelease() => await ReleasesHandler.GetLatestVersion();
}