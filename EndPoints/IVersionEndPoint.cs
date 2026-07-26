using Gridly.Models;

namespace Gridly.EndPoints;

public interface IVersionEndPoint
{
    public Task<(bool Success, VersionModel? Version)> GetLatestVersion();
}
