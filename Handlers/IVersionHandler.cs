
using Gridly.Models;

namespace Gridly.Handlers;

public interface IVersionHandler
{
    public Task<VersionModel> GetVersionAsync();
}