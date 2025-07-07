using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class BatchEditComponentCommand : List<ComponentModel>, IRequest<IResult> {}