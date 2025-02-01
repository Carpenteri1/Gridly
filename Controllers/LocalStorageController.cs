using Gridly.Handler;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/layout")]
public class LayoutController : ControllerBase {
           
    [HttpPost("save")]
    public IResult Save([FromBody] ComponentModel newComponent) 
        => LocalComponentHandler.Save(newComponent);

    [HttpGet("get")]
    public async Task<ComponentModel[]?> Get() => 
        await LocalComponentHandler.Get();
    
    [HttpGet("getbyid/{Id}")]
    public async Task<ComponentModel?> GetById(int Id) => 
        await LocalComponentHandler.GetById(Id);
    
    [HttpPost("edit")]
    public IResult Edit([FromBody] ComponentModel component) => 
        LocalComponentHandler.Edit(component);
    
    [HttpDelete("delete/{Id}")]
    public IResult Delete(int Id) 
        => LocalComponentHandler.Delete(Id);
}