using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class BatchEditRowColumnCommand : List<ColumnRowModel>, IRequest<IResult> {}