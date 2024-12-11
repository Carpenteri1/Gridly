using Gridly.Handler;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/layout")]
public class LayoutController : ControllerBase {
           
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] ComponentModel[] newComponent) 
        => LocalComponentHandler.Save(newComponent);

    [HttpGet("get")]
    public async Task<ComponentModel[]> Get() => 
        await LocalComponentHandler.Get();
    
    [HttpDelete("delete/{Id}")]
    public IResult Delete(int Id) 
        => LocalComponentHandler.Delete(Id);
}