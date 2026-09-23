using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Factories;

public static class ColumnRowFactory
{
    public static ColumnRowModel Create(ColumnRowEntity entity)
        => new()
        {
            Id = entity.Id,
            RowPosition = entity.RowPosition,
            RowWidth = entity.RowWidth,
            Cards = entity.Cards?.Select(CardFactory.Create) ?? []
        };
    
    public static ColumnRowEntity Create(ColumnRowModel model)
        => new()
        {
            Id = model.Id,
            RowPosition = model.RowPosition,
            RowWidth = model.RowWidth,
        };


    public static IEnumerable<ColumnRowModel> CreateMany(IEnumerable<ColumnRowEntity> entities)
        => entities.Select(Create);
}
