using Gridly.Models;

namespace Gridly.Services;

public interface IIconRepository
{
    public Task<IconModel> Insert(IconModel icon);
    public Task<IconModel> GetById(int Id);
    public Task<IconModel> GetByFullName(IconModel icon);
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components);
    public Task<bool> Delete(int Id);
}