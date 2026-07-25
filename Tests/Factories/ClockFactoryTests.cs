using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class ClockFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToClockModel()
    {
        var dto = new ClockDtoModel
        {
            TimeZone = "Europe/Stockholm",
            UtcOffsetSeconds = 7200,
            DstActive = true,
        };

        var result = ClockFactory.Create(dto);

        Assert.Equal("Europe/Stockholm", result.TimeZone);
        Assert.Equal(7200, result.UtcOffsetSeconds);
        Assert.True(result.DstActive);
    }
}
