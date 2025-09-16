using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class SaveVersionCommand : VersionModel, IRequest<IResult> {}