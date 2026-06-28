using Gridly.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RowController(IMediator meditor) : ControllerBase
{
    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] SaveColumnRowCommands commands) =>
        await meditor.Send(commands) is null ? 
            Results.BadRequest() : Results.Ok();
    
    [HttpGet("get")]
    public async Task<IResult> Get() =>
        await meditor.Send(new GetAllRowColumnsCommands());
}