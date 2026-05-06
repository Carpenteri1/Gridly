using Gridly.Models;

namespace Gridly.Services;

public interface ISettingsRepository
{
    public Task<SettingsModel> Insert(SettingsModel settings);
    public Task<SettingsModel> Edit(SettingsModel settings);
    public Task<bool> Delete(int Id);
}