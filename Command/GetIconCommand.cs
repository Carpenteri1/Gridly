using Gridly.Models;
using MediatR;

namespace Gridly.Command;
public class GetIconCommand : IconModel, IRequest<IResult> {}