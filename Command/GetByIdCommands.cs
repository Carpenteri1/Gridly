using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetByIdComponentCommands : IRequest<ComponentModel>
{
    public int Id { get; set; }
}