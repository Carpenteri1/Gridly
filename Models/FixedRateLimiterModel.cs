namespace Gridly.Models;
public class FixedRateLimiterModel
{
    public int QueueLimit { get; private set; } = 1;
    public int Limit { get; private set; } = 1;
    public TimeSpan Window { get; set; }
}