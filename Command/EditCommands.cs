using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class EditComponentCommand : EditComponentModel, IRequest<IResult> {}