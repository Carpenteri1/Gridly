using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetLatestVersionQuery : VersionModel, IRequest<IResult> {}
