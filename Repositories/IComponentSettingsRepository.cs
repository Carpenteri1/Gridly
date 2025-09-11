using Gridly.Models;

namespace Gridly.Services;

public interface IComponentSettingsRepository
{
    public Task<bool> Insert(ComponentSettingsModel settings);
    public Task<bool> Delete(int Id);
}