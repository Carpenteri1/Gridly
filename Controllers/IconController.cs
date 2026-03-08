using Gridly.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;


[ApiController]
[Route("/api/[controller]")]
public class IconController(IMediator mediator) : Controller
{
    [HttpGet("get")]
    public async Task<IResult> Get() =>
        await mediator.Send(new GetIconsCommand());
}
