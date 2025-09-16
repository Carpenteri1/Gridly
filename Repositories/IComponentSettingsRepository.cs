using Gridly.Models;

namespace Gridly.Services;

public interface IComponentSettingsRepository
{
    public Task<ComponentSettingsModel> Insert(ComponentSettingsModel settings);
    public Task<ComponentSettingsModel> Edit(ComponentSettingsModel settings);
    public Task<bool> Delete(int Id);
}