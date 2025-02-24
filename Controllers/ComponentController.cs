using Gridly.Handlers;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ComponentController(IComponentHandler handler) : ControllerBase
{
    [HttpPost("save")]
    public IResult Save([FromBody] ComponentModel newComponent) => handler.Save(newComponent);

    [HttpGet("get")]
    public async Task<ComponentModel[]?> Get() => await handler.GetAsync();
    
    [HttpGet("getbyid/{Id}")]
    public async Task<ComponentModel?> GetById(int Id) => await handler.GetByIdAsync(Id);
    
    [HttpPost("edit")]
    public IResult Edit([FromBody] EditComponentForm editComponentForm) => handler.Edit(editComponentForm);
    
    [HttpDelete("delete/{Id}")]
    public IResult Delete(int Id) => handler.Delete(Id);
}