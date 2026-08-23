using Gridly.Models;
using MediatR;

namespace Gridly.Commands;

public class BatchSaveColumnRowCommands : List<ColumnRowModel>, IRequest<IResult> {}