using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetVersionQuery : VersionModel, IRequest<IResult> {}
