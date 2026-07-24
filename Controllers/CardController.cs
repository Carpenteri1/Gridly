using Gridly.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CardController(IMediator meditor) : ControllerBase
{
    [HttpGet("get")]
    public async Task<IResult> Get() =>
        await meditor.Send(new GetAllCardQuery());
}