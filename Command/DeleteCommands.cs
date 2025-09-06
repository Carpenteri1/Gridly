using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class DeleteComponentCommand : ComponentModel, IRequest<IResult>
{
    public int Id { get; set; }
}