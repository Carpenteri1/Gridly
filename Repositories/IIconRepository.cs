using Gridly.Models;

namespace Gridly.Services;

public interface IIconRepository
{
    public Task<IconModel> Insert(IconModel icon);
    public List<string> FindUnusedIcons(IEnumerable<CardModel> cards);
    public Task<bool> Delete(int Id);
}