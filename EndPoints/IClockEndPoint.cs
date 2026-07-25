using Gridly.Models;

namespace Gridly.EndPoints;

public interface IClockEndPoint
{
    public Task<(ClockFetchStatus Status, ClockModel? Clock)> Get(string timeZone);
}
