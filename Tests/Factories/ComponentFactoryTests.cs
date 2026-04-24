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
}
