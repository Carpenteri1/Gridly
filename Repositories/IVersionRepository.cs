using Gridly.Models;

namespace Gridly.Repositories;

public interface IVersionRepository
{
    public Task<VersionModel> GetVersionAsync();
    public Task<VersionModel> SaveVersionAsync();
}