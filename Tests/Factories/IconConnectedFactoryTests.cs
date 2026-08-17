using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class IconConnectedFactoryTests
{
    [Fact]
    public void Create_MapsCardAndIconIds()
    {
        var result = IconConnectedFactory.Create(4, 9);

        Assert.Equal(4, result.CardId);
        Assert.Equal(9, result.IconId);
    }
}
