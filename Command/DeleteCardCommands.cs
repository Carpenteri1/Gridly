using MediatR;

namespace Gridly.Command;

public class DeleteCardCommand : IRequest<IResult>
{
    public int Id { get; set; }
}