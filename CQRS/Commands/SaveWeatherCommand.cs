using Gridly.Models;
using MediatR;

namespace Gridly.Commands;

public class SaveWeatherCommand : IRequest<IResult> {}