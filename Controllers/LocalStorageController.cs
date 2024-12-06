using Gridly.Handler;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/layout")]
public class LayoutController : ControllerBase {
           
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] ComponentModel newComponent) 
        => LocalComponentHandler.Save(newComponent);
    
    [HttpPost("Remove")]
    public async Task<IResult> Remove(int componentId) 
        => Results.Ok();
    
    [HttpGet("Get")]
    public async Task<IResult> Get() 
        => Results.Ok();
    
}