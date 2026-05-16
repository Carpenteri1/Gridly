using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class BatchEditCardCommand : List<CardModel>, IRequest<IResult> {}