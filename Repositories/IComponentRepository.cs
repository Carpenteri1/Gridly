using Gridly.Models;

namespace Gridly.Services;

public interface IComponentRepository
{
    public bool Save(IEnumerable<ComponentModel> component);
    public Task<IEnumerable<ComponentModel>?> Get();
    public Task<ComponentModel?> GetById(int id);
    public bool UploadIcon(IconModel iconData);
    public bool DeleteIcon(string name, string type);
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components);
}