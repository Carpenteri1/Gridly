using Gridly.Factories;
using Gridly.Models;

namespace Gridly.Tests.Factories;

public class SettingsFactoryTests
{
    [Fact]
    public void Create_WithNullInput_ReturnsDefaults()
    {
        var result = SettingsFactory.Create(null);

        Assert.Equal(250, result.Width);
        Assert.Equal(250, result.Height);
        Assert.False(result.TitleHidden);
        Assert.False(result.ImageHidden);
    }

    [Fact]
    public void Create_WithValues_PreservesProvidedValues()
    {
        var settings = new SettingsModel
        {
            Width = 500,
            Height = 300,
            TitleHidden = true,
            ImageHidden = true
        };

        var result = SettingsFactory.Create(settings);

        Assert.Equal(500, result.Width);
        Assert.Equal(300, result.Height);
        Assert.True(result.TitleHidden);
        Assert.True(result.ImageHidden);
    }
}
