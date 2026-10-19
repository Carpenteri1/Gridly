using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Extension;

public static class ColumnRowQueryExtensions
{
    public static IQueryable<ColumnRowEntity> SelectMatchingRows(
        this IQueryable<ColumnRowEntity> rowEntities, 
        IEnumerable<ColumnRowModel> columnRows)
    {
        var ids = columnRows.Select(r => r.Id).ToArray();
        return rowEntities.Where(r => ids.Contains(r.Id));
    }
}
