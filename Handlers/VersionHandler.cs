using Gridly.Models;
using Gridly.Repositories;

namespace Gridly.Handlers;

public class VersionHandler(IVersionRepository versionRepository) : IVersionHandler
{
    public async Task<VersionModel> GetVersionAsync() => await versionRepository.GetVersionAsync();
}