using Gridly.Models;

namespace Gridly.Repositories;

public interface IWidgetRepository
{
    public Task<IEnumerable<WidgetModel>> Get();
}