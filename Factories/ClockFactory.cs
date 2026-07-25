using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public class ClockFactory
{
    public static ClockModel Create(ClockDtoModel dto)
        => new()
        {
            TimeZone = dto.TimeZone,
            UtcOffsetSeconds = dto.UtcOffsetSeconds,
            DstActive = dto.DstActive,
        };
}
