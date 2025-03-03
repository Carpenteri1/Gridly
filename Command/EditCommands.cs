using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class EditComponentCommand : EditComponentForm, IRequest<IResult>
{
}