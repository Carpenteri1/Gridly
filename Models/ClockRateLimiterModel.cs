namespace Gridly.Models;
public class ClockRateLimiterModel
{
    public int QueueLimit { get; private set; } = 0;
    public int Limit { get; private set; } = 2;
    public int TokensPerPeriod { get; private set; } = 2;
    public TimeSpan Window { get; private set; } = TimeSpan.FromMinutes(30);
}
