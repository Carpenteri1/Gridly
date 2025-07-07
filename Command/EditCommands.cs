using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class EditComponentCommand : EditFormModel, IRequest<IResult>
{
}