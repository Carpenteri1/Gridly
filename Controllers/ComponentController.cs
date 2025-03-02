using Gridly.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ComponentController(IMediator meditor) : ControllerBase
{
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] SaveCommand command) =>
        await meditor.Send(command) is null ? 
            Results.BadRequest() : Results.Ok();

    [HttpGet("get")]
    public async Task<ComponentModel[]> Get() =>
        await meditor.Send(new GetAllCommand());
    
    [HttpGet("getbyid/{Id}")]
    public async Task<ComponentModel> GetById(int Id) => 
        await meditor.Send(new GetByIdCommand { Id = Id });

    [HttpPost("edit")]
    public async Task<IResult> Edit([FromBody] EditCommand command) => 
        await meditor.Send(command);

    [HttpDelete("delete/{Id}")]
    public async Task<IResult> Delete(int Id) => 
        await meditor.Send(new DeleteCommand {Id = Id});
}