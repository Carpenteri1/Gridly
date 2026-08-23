using Gridly.Models;

namespace Gridly.Repositories.Interfaces;

public interface IColumnRowRepository
{
    public Task<ColumnRowModel> Insert(ColumnRowModel columnRow);
    public Task<IEnumerable<ColumnRowModel>?> Get();
    public Task<bool> BatchDelete(IEnumerable<ColumnRowModel> columnRows);
    public Task<bool> BatchEdit(IEnumerable<ColumnRowModel> columnRows);
}