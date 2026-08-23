using Gridly.Models;

namespace Gridly.Repositories.Interfaces;

public interface IIconRepository
{
    public Task<IconModel> Insert(IconModel icon);
    public Task<bool> Delete(int Id);
}