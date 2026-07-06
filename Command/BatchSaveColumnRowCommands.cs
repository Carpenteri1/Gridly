using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class BatchSaveColumnRowCommands : List<ColumnRowModel>, IRequest<IResult> {}