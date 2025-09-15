using Gridly.Models;

namespace Gridly.Services;

public interface IComponentRepository
{
    public Task<ComponentModel> Insert(ComponentModel component);
    public Task<bool> Edit(ComponentModel component);
    public Task<bool> BatchEdit(IEnumerable<ComponentModel>? components);
    public Task<IEnumerable<ComponentModel>?> Get();
    public Task<ComponentModel> GetById(int Id);
    public Task<bool> Delete(ComponentModel component);
}