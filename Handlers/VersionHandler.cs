using Gridly.Models;
using Gridly.Repositories;

namespace Gridly.Handlers;

public class VersionHandler(IVersionRepository versionRepository) : 
    IVersionHandler<VersionModel>
{
    public async Task<VersionModel> Handler()
    {
        var version = await versionRepository.GetVersionAsync();
        return version;
    }
}