using Gridly.Command;
using Gridly.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CardController(IMediator meditor) : ControllerBase
{
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] SaveCardCommand commands) =>
        await meditor.Send(commands) is null ? 
            Results.BadRequest() : Results.Ok();

    [HttpGet("get")]
    public async Task<IResult> Get() =>
        await meditor.Send(new GetAllCardCommand());
    
    [HttpGet("getbyid/{Id}")]
    public async Task<CardModel> GetById(int Id) => 
        await meditor.Send(new GetCardByIdCommands { Id = Id });

    [HttpPost("edit")]
    public async Task<IResult> Edit([FromBody] EditCardCommand command) => 
        await meditor.Send(command);
    
    [HttpPost("batch/edit")]
    public async Task<IResult> Edit([FromBody] BatchEditCardCommand command) => 
        await meditor.Send(command);

    [HttpDelete("delete/{id}")]
    public async Task<IResult> Delete(int id) => 
        await meditor.Send(new DeleteCardCommand {Id = id});
}