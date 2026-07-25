using Gridly.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Configuration;

public static class RateLimiterPolicySettings
{
    public const string VersionPolicy = "version";
    public const string WeatherPolicy = "weather";
    public const string ClockPolicy = "clock";

    public static async Task<IServiceCollection> AddTokenBucketRateLimiter(this IServiceCollection services) {

        var versionRate = new VersionRateLimiterModel();
        var weatherRate = new WeatherRateLimiterModel();
        var clockRate = new ClockRateLimiterModel();

        services.AddRateLimiter(_ => _
            .AddTokenBucketLimiter(VersionPolicy, opt =>
            {
                opt.TokenLimit = versionRate.Limit;
                opt.QueueLimit = versionRate.QueueLimit;
                opt.ReplenishmentPeriod = versionRate.Window;
                opt.TokensPerPeriod = versionRate.TokensPerPeriod;
                opt.AutoReplenishment = true;
            })
            .AddTokenBucketLimiter(WeatherPolicy, opt =>
            {
                opt.TokenLimit = weatherRate.Limit;
                opt.QueueLimit = weatherRate.QueueLimit;
                opt.ReplenishmentPeriod = weatherRate.Window;
                opt.TokensPerPeriod = weatherRate.TokensPerPeriod;
                opt.AutoReplenishment = true;
            })
            .AddTokenBucketLimiter(ClockPolicy, opt =>
            {
                opt.TokenLimit = clockRate.Limit;
                opt.QueueLimit = clockRate.QueueLimit;
                opt.ReplenishmentPeriod = clockRate.Window;
                opt.TokensPerPeriod = clockRate.TokensPerPeriod;
                opt.AutoReplenishment = true;
            }));
        return services;
    }
    
    public static IApplicationBuilder UseTokenBucketRateLimiter(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        app.UseMiddleware<TokenBucketRateLimiterMiddleware>();
        return app;
    }
}