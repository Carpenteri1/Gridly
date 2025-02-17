using Gridly.DTOs;
using Gridly.Handler;
using Gridly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ComponentController : ControllerBase {
           
    [HttpPost("save")]
    public IResult Save([FromBody] ComponentModel newComponent) 
        => SaveComponentHandler.Save(newComponent);

    [HttpGet("get")]
    public async Task<ComponentModel[]?> Get() => 
        await GetComponentHandler.Get();
    
    [HttpGet("getbyid/{Id}")]
    public async Task<ComponentModel?> GetById(int Id) => 
        await GetComponentHandler.GetById(Id);
    
    [HttpPost("edit")]
    public IResult Edit([FromBody] EditComponentDto editData) => 
        EditComponentHandler.Edit(editData);
    
    [HttpDelete("delete/{Id}")]
    public IResult Delete(int Id) 
        => DeleteComponentHandler.Delete(Id);
}