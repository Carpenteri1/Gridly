using Gridly.Models;

namespace Gridly.Services;

public interface IComponentRepository
{
    public bool Insert(ComponentModel component);
    public bool Insert(IEnumerable<ComponentModel>? components);
    public Task<IEnumerable<ComponentModel>?> Get();
    public Task<ComponentModel> GetById(int id);
    public bool Delete(ComponentModel component);
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components);
}