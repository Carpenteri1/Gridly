using Gridly.Models;

namespace Gridly.Services;

public interface ICardRepository
{
    public Task<CardModel> Insert(CardModel Card);
    public Task<bool> Edit(CardModel Card);
    public Task<bool> BatchEdit(IEnumerable<CardModel>? cards);
    public Task<IEnumerable<CardModel>?> Get();
    public Task<CardModel> GetById(int Id);
    public Task<bool> Delete(int Id);
}