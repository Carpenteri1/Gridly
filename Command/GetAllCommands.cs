using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetAllComponentCommand : IRequest<ComponentModel[]>
{
}
public class GetAllVersionCommand : IRequest<VersionModel>
{
}