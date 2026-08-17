using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public static class ColumnRowFactory
{
    public static ColumnRowModel Create(ColumnRowDtoModel dto) 
        => new()
        {
            Id = dto.Id,
            RowPosition = dto. RowPosition,
            RowWidth = dto.RowWidth,
            Cards = []
        };

    public static IEnumerable<ColumnRowModel> CreateMany(IEnumerable<ColumnRowDtoModel> dtos) 
        => dtos.Select(Create);
}