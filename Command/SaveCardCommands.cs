using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class SaveCardCommand : CardModel, IRequest<IResult>
{
}