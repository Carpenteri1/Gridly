using Gridly.Dtos;
using Gridly.Factories;
using Gridly.Models;

namespace Gridly.Tests.Factories;

public class ColumnRowFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToColumnRowModel()
    {
        var dto = new ColumnRowDtoModel
        {
            Id = 4,
            RowPosition = 2,
            RowWidth = 12,
            Cards = Array.Empty<CardModel>()
        };

        var result = ColumnRowFactory.Create(dto);

        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.RowPosition, result.RowPosition);
        Assert.Equal(dto.RowWidth, result.RowWidth);
    }

    [Fact]
    public void Create_AlwaysInitializesEmptyCardsList()
    {
        var dto = new ColumnRowDtoModel
        {
            Id = 1,
            RowPosition = 0,
            RowWidth = 12,
            Cards = new[] { new CardModel { Id = 1, Name = "", Url = "", IconUrl = "" } }
        };

        var result = ColumnRowFactory.Create(dto);

        Assert.Empty(result.Cards);
    }

    [Fact]
    public void CreateMany_PreservesOrderAndCount()
    {
        var dtos = new[]
        {
            new ColumnRowDtoModel { Id = 1, RowPosition = 0, RowWidth = 12, Cards = Array.Empty<CardModel>() },
            new ColumnRowDtoModel { Id = 2, RowPosition = 1, RowWidth = 6, Cards = Array.Empty<CardModel>() }
        };

        var result = ColumnRowFactory.CreateMany(dtos).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }
}
