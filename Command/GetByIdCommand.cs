using MediatR;

namespace Gridly.Models;

public class GetByIdCommand : IRequest<ComponentModel>
{
    public int Id { get; set; }
}