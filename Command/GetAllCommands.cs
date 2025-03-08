using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetAllComponentCommand : IRequest<ComponentModel[]>
{
}
public class GetAllLatestVersionCommand : IRequest<VersionModel>
{
}

public class GetAllCurrentVersionCommand : IRequest<VersionModel>
{
}