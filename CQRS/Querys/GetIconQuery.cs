using Gridly.Models;
using MediatR;

namespace Gridly.Command;
public class GetIconQuery : IconModel, IRequest<IResult> {}