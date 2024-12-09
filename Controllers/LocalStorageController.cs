using Gridly.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/layout")]
public class LayoutController : ControllerBase {
           
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] dynamic[] newComponent) 
        => LocalComponentHandler.Save(newComponent);

    [HttpGet("get")]
    public async Task<dynamic[]> Get()
    {
        var obj = await LocalComponentHandler.Get();
        return obj;
    }
    
    [HttpPost("Remove")]
    public async Task<IResult> Remove(int componentId) 
        => Results.Ok();
}