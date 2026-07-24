using Gridly.Models;
using MediatR;

namespace Gridly.Querys;
public class GetIconQuery : IconModel, IRequest<IResult> {}