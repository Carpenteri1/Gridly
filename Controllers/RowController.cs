using Gridly.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RowController(IMediator meditor) : ControllerBase
{
    [HttpPost("batchSave")]
    public async Task<IResult> BatchSave([FromBody] BatchSaveColumnRowCommands commands) =>
        await meditor.Send(commands);
    
    [HttpGet("get")]
    public async Task<IResult> Get() =>
        await meditor.Send(new GetAllRowColumnsQuery());
}
