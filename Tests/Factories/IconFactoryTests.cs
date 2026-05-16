using Gridly.Factories;
using Gridly.Models;

namespace Gridly.Tests.Factories;

public class IconFactoryTests
{
    [Fact]
    public void Create_WithNullInput_ReturnsDefaultIcon()
    {
        var result = IconFactory.Create(null);

        Assert.Equal(string.Empty, result.Name);
        Assert.Equal(string.Empty, result.Type);
        Assert.Equal(string.Empty, result.Base64Data);
        Assert.Equal("add_box", result.MaterialIcon);
    }

    [Fact]
    public void Create_WithValues_PreservesProvidedValues()
    {
        var icon = new IconModel
        {
            Name = "grid",
            Type = "svg",
            Base64Data = "Zm9v",
            MaterialIcon = "dashboard"
        };

        var result = IconFactory.Create(icon);

        Assert.Equal(icon.Name, result.Name);
        Assert.Equal(icon.Type, result.Type);
        Assert.Equal(icon.Base64Data, result.Base64Data);
        Assert.Equal(icon.MaterialIcon, result.MaterialIcon);
    }

    [Fact]
    public void Create_WithNullMaterialIcon_FallsBackToDefault()
    {
        var icon = new IconModel
        {
            Name = "grid",
            Type = "svg",
            Base64Data = "Zm9v",
            MaterialIcon = null!
        };

        var result = IconFactory.Create(icon);

        Assert.Equal("add_box", result.MaterialIcon);
    }
}
