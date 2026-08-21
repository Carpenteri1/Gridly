using Gridly.Models;

namespace Gridly.Repositories.Interfaces;

public interface ISettingsRepository
{
    public Task<SettingsModel> Insert(SettingsModel settings);
    public Task<SettingsModel> Edit(SettingsModel settings);
    public Task<bool> Delete(int Id);
}