using Gridly.Models;
using MediatR;

namespace Gridly.Querys;

public class GetVersionQuery : VersionModel, IRequest<IResult> {}
