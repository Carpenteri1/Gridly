using Gridly.Dtos;

namespace Gridly.Repositories;

public interface IIconConnectedRepository
{
    public Task<IconConnectedDtoModel> GetById(int? componentId, int? iconId);
    public Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? componentId, int? iconId);
    public Task<bool> Insert(IconConnectedDtoModel model);
    public Task<bool> Delete(int componentId);
}