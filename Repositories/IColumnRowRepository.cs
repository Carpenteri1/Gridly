using Gridly.Models;

namespace Gridly.Repositories;

public interface IColumnRowRepository
{
    public Task<ColumnRowModel> Insert(ColumnRowModel columnRow);
    public Task<IEnumerable<ColumnRowModel>> Get();
    public Task<IEnumerable<ColumnRowModel>> BatchEdit(IEnumerable<ColumnRowModel> columnRows);
}