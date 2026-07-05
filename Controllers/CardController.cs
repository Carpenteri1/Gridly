using Gridly.Command;
using Gridly.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CardController(IMediator meditor) : ControllerBase
{
    [HttpGet("get")]
    public async Task<IResult> Get() =>
        await meditor.Send(new GetAllCardCommand());
}