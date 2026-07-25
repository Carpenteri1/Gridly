namespace Gridly.Models;
public class ApiKeyRateLimiterModel
{
    public int QueueLimit { get; private set; } = 0;
    public int Limit { get; private set; } = 5;
    public int TokensPerPeriod { get; private set; } = 5;
    public TimeSpan Window { get; private set; } = TimeSpan.FromMinutes(15);
}
