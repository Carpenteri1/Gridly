using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class SaveComponentCommand : ComponentModel, IRequest<IResult>
{
}