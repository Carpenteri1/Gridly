using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handlers;

public interface IComponentHandler
{
    public IResult Save(ComponentModel newComponent);
    public IResult Delete(int componentId);
    public Task<ComponentModel[]?> GetAsync();
    public Task<ComponentModel?> GetByIdAsync(int Id);
    public IResult Edit(EditComponentForm editedComponentForm);
}