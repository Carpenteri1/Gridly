using Gridly.Command;
using Gridly.Models;
using Gridly.Repositories;
using MediatR;

namespace Gridly.Handlers;

public class VersionHandler(IVersionRepository versionRepository) : 
    IRequestHandler<GetAllVersionCommand,VersionModel>
{
    public async Task<VersionModel> Handle(GetAllVersionCommand request, CancellationToken cancellationToken)
    {
        var version = await versionRepository.GetVersionAsync();
        return version;
    }
}