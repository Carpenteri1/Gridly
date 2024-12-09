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
    public async Task<dynamic[]> Get() => 
        await LocalComponentHandler.Get();
    
    [HttpPost("delete")]
    public async Task<IResult> Delete(int componentId) 
        => await LocalComponentHandler.Delete(componentId);
}