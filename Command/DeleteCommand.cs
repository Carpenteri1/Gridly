using MediatR;

namespace Gridly.Models;

public class DeleteCommand : IRequest<IResult>
{
    public int Id { get; set; }
}