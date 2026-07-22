using MediatR;

namespace Gridly.Command;

public class GetWeatherCommand : IRequest<IResult> {}