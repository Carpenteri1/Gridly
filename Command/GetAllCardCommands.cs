using MediatR;

namespace Gridly.Command;

public class GetAllCardCommand : IRequest<IResult> {}