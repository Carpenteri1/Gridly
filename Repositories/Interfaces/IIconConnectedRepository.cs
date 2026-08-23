using Gridly.Dtos;

namespace Gridly.Repositories.Interfaces;

public interface IIconConnectedRepository
{
    public Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? cardId, int? iconId);
    public Task<IconConnectedDtoModel> Insert(IconConnectedDtoModel model);
    public Task<bool> Delete(int cardId);
}