using Gridly.Models;

namespace Gridly.Repositories.Interfaces;

public interface IWidgetRepository
{
    public Task<IEnumerable<WidgetModel>> Get();
}