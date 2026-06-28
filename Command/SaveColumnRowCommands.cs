using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class SaveColumnRowCommands : ColumnRowModel, IRequest<IResult> { }