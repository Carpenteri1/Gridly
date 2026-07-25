using Gridly.Commands;
using Gridly.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProviderKeysController(IMediator mediator) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IResult> GetStatus([FromQuery] GetProviderKeyStatusQuery query) => await mediator.Send(query);

    [HttpPost("save")]
    public async Task<IResult> Save([FromBody] SaveProviderKeyCommand command) => await mediator.Send(command);
}
