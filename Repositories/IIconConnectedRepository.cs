using Gridly.Dtos;

namespace Gridly.Repositories;

public interface IIconConnectedRepository
{
    public Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? componentId, int? iconId);
    public Task<IconConnectedDtoModel> Insert(IconConnectedDtoModel model);
    public Task<bool> Delete(int componentId);
}