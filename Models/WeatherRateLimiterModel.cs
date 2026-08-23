namespace Gridly.Models;
public class WeatherRateLimiterModel
{
    public int QueueLimit { get; private set; } = 1;
    public int Limit { get; private set; } = 1;
    public int TokensPerPeriod { get; private set; } = 1;
    public TimeSpan Window { get; private set; } = TimeSpan.FromMinutes(30);
}