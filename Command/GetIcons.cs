using Gridly.Models;
using MediatR;

namespace Gridly.Command;
public class GetIconsCommand : List<IconModel>, IRequest<IResult> {}