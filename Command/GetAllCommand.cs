using MediatR;

namespace Gridly.Models;

public class GetAllCommand : IRequest<ComponentModel[]>
{
    public int ComponentId { get; set; }
}