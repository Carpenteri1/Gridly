using Gridly.Models;

namespace Gridly.Repositories;

public interface IVersionRepository
{
    public (bool, VersionModel) GetVersion();
    public (bool, VersionModel) SaveVersion(VersionModel version);
}