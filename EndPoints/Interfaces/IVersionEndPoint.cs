using Gridly.Models;

namespace Gridly.EndPoints.Interfaces;

public interface IVersionEndPoint
{
    public Task<(bool, VersionModel?)> GetLatestVersion();
    public Task<(bool, VersionModel?)> GetVersion();
}