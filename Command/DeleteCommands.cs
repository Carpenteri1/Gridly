using MediatR;

namespace Gridly.Command;

public class DeleteComponentCommand : IRequest<IResult>
{
    public int Id { get; set; }
}