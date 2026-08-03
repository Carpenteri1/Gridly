using Gridly.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Configuration;

public static class RateLimiterPolicySettings
{
    public const string VersionProviderPolicy = "version";
    public const string WeatherProviderPolicy = "weather";
    public const string RemoteProviderKeyPolicy = "remoteProviderKey";
    public const string ClockProviderPolicy = "clock";

    public static async Task<IServiceCollection> AddTokenBucketRateLimiter(this IServiceCollection services) {

        var versionRate = new VersionRateLimiterModel();
        var weatherRate = new WeatherRateLimiterModel();
        var providerKeyRate = new ProviderKeyRateLimiterModel();
        var clockRate = new ClockRateLimiterModel();

        services.AddRateLimiter(_ => _
            .AddTokenBucketLimiter(VersionProviderPolicy, opt =>
            {
                opt.TokenLimit = versionRate.Limit;
                opt.QueueLimit = versionRate.QueueLimit;
                opt.ReplenishmentPeriod = versionRate.Window;
                opt.TokensPerPeriod = versionRate.TokensPerPeriod;
                opt.AutoReplenishment = true;
            })
            .AddTokenBucketLimiter(WeatherProviderPolicy, opt =>
            {
                opt.TokenLimit = weatherRate.Limit;
                opt.QueueLimit = weatherRate.QueueLimit;
                opt.ReplenishmentPeriod = weatherRate.Window;
                opt.TokensPerPeriod = weatherRate.TokensPerPeriod;
                opt.AutoReplenishment = true;
            })
            .AddTokenBucketLimiter(RemoteProviderKeyPolicy, opt =>
            {
                opt.TokenLimit = providerKeyRate.Limit;
                opt.QueueLimit = providerKeyRate.QueueLimit;
                opt.ReplenishmentPeriod = providerKeyRate.Window;
                opt.TokensPerPeriod = providerKeyRate.TokensPerPeriod;
                opt.AutoReplenishment = true;
            })
            .AddTokenBucketLimiter(ClockProviderPolicy, opt =>
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