using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Extension;

public static class ColumnRowQueryExtensions
{
    public static IQueryable<ColumnRowEntity> SelectMatchingRows(
        this IQueryable<ColumnRowEntity> row, 
        IEnumerable<ColumnRowModel> columnRows)
    {
        var ids = columnRows.Select(r => r.Id).ToArray();
        return row.Where(r => ids.Contains(r.Id));
    }
}
