using Gridly.Models;
using MediatR;

namespace Gridly.Querys;

public class GetLatestVersionQuery : VersionModel, IRequest<IResult> {}
