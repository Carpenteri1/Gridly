using Gridly.Models;

namespace Gridly.Repositories;

public interface IClockRepository
{
    public Task<(ClockModel? Clock, DateTime? FetchedAt)> Get(string location);
    public Task<bool> Upsert(string location, ClockModel clock);
}
