using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/layout")]
public class LayoutController : ControllerBase {
           
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] BasicComponentModel[] flexItems)
    {                                          
        return Results.Ok();
    }

    [HttpPost("Remove")]
    public async Task<IResult> Remove(int componentId)
    {
        return Results.Ok();
    }

    [HttpGet("Get")]
    public async Task<IResult> Get()
    {
        return Results.Ok();
    }
}