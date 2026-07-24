using Gridly.Models;
using MediatR;

namespace Gridly.Querys;

public class BatchSaveColumnRowCommands : List<ColumnRowModel>, IRequest<IResult> {}