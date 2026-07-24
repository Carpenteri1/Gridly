using MediatR;

namespace Gridly.Command;

public class GetAllRowColumnsQuery : IRequest<IResult> {}