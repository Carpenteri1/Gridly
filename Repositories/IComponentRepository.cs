using Gridly.Models;

namespace Gridly.Services;

public interface IComponentRepository
{
    public bool ReadToJsonFile(List<ComponentModel> newComponent);
    public Task<ComponentModel[]?> ReadAllFromJsonFile();
    public Task<ComponentModel?> ReadByIdFromJsonFile(int id);
    public bool WriteIconToFolder(IconModel iconData);
    public bool DeleteIconFromFolder(IconModel iconData);
    public bool IconExcistOnOtherComponent(List<ComponentModel> componentModels, IconModel iconData);
}