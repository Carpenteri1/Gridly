using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class ComponentFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToComponentModel()
    {
        var dto = new ComponentDtoModel
        {
            ComponentId = 12,
            IndexPosition = 3,
            ComponentName = "Docs",
            Url = "https://example.test",
            IconUrl = "/icons/docs.svg"
        };

        var result = ComponentFactory.Create(dto);

        Assert.Equal(dto.ComponentId, result.Id);
        Assert.Equal(dto.IndexPosition, result.IndexPosition);
        Assert.Equal(dto.ComponentName, result.Name);
        Assert.Equal(dto.Url, result.Url);
        Assert.Equal(dto.IconUrl, result.IconUrl);
        Assert.NotNull(result.IconData);
        Assert.NotNull(result.ComponentSettings);
    }

    [Fact]
    public void CreateMany_PreservesOrderAndCount()
    {
        var dtos = new[]
        {
            new ComponentDtoModel { ComponentId = 1, ComponentName = "First" },
            new ComponentDtoModel { ComponentId = 2, ComponentName = "Second" }
        };

        var result = ComponentFactory.CreateMany(dtos).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("First", result[0].Name);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Second", result[1].Name);
    }

    [Fact]
    public void Create_MapsIconAndComponentSettings()
    {
        var dto = new ComponentDtoModel
        {
            ComponentId = 8,
            IconId = 21,
            IconName = "grid",
            Type = "svg",
            Base64Data = "Zm9v",
            MaterialIcon = "dashboard",
            ComponentSettingsId = 13,
            Width = 500,
            Height = 300,
            TitleHidden = true,
            ImageHidden = false
        };

        var result = ComponentFactory.Create(dto);

        Assert.NotNull(result.IconData);
        Assert.Equal(21, result.IconData.Id);
        Assert.Equal("grid", result.IconData.Name);
        Assert.Equal("svg", result.IconData.Type);
        Assert.Equal("Zm9v", result.IconData.Base64Data);
        Assert.Equal("dashboard", result.IconData.MaterialIcon);

        Assert.NotNull(result.ComponentSettings);
        Assert.Equal(13, result.ComponentSettings.Id);
        Assert.Equal(8, result.ComponentSettings.ComponentId);
        Assert.Equal(500, result.ComponentSettings.Width);
        Assert.Equal(300, result.ComponentSettings.Height);
        Assert.True(result.ComponentSettings.TitleHidden);
        Assert.False(result.ComponentSettings.ImageHidden);
    }
}
