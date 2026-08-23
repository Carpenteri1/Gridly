using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Factories;

public static class ColumnRowFactory
{
    public static ColumnRowModel Create(RowColumnEntity entity)
        => new()
        {
            Id = entity.Id,
            RowPosition = entity.RowPosition,
            RowWidth = entity.RowWidth,
            Cards = []
        };

    public static IEnumerable<ColumnRowModel> CreateMany(IEnumerable<RowColumnEntity> entities)
        => entities.Select(Create);
}
