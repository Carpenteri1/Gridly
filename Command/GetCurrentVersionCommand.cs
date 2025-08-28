using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetCurrentVersionCommand : VersionModel, IRequest<IResult> {}
