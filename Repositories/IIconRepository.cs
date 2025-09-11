using Gridly.Models;

namespace Gridly.Services;

public interface IIconRepository
{
    public Task<bool> Insert(IconModel icon);
    public Task<IconModel> GetById(int Id);
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components);
    public Task<bool> Delete(int Id);
    public Task<IconModel> Edit(int Id);
}