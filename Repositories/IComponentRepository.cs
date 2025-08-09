using Gridly.Models;

namespace Gridly.Services;

public interface IComponentRepository
{
    public bool Save(IEnumerable<ComponentModel> newComponent);
    public Task<IEnumerable<ComponentModel>?> Get();
    public Task<ComponentModel?> GetById(int id);
    public bool UploadIcon(IconModel iconData);
    public bool DeleteIcon(string name, string type);
    public bool DeleteUnusedIcons(IEnumerable<ComponentModel> componentModels);
    public bool IconDuplicate(IEnumerable<ComponentModel> componentModels, IconModel iconData);
}