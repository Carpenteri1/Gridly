using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetVersionCommand : VersionModel, IRequest<IResult> {}
