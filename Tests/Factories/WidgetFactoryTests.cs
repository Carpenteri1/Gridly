using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class WidgetFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToWidgetModel()
    {
        var dto = new WidgetDtoModel
        {
            Id = 3,
            WidgetType = "Weather",
            Label = "Weather widget",
            Description = "Shows the local forecast",
            Icon = "cloud"
        };

        var result = WidgetFactory.Create(dto);

        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.WidgetType, result.WidgetType);
        Assert.Equal(dto.Label, result.Label);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Icon, result.Icon);
    }

    [Fact]
    public void CreateMany_PreservesOrderAndCount()
    {
        var dtos = new[]
        {
            new WidgetDtoModel { Id = 1, WidgetType = "Empty", Label = "Empty", Description = "", Icon = "" },
            new WidgetDtoModel { Id = 2, WidgetType = "Custom", Label = "Custom", Description = "", Icon = "" }
        };

        var result = WidgetFactory.CreateMany(dtos).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal("Empty", result[0].WidgetType);
        Assert.Equal("Custom", result[1].WidgetType);
    }
}
