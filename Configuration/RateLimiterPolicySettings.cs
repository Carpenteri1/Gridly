using Gridly.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Configuration;

public static class RateLimiterPolicySettings
{
    public const string VersionProviderPolicy = "version";
    public const string WeatherProviderPolicy = "weather";

    public static async Task<IServiceCollection> AddTokenBucketRateLimiter(this IServiceCollection services) {

        var versionRate = new VersionRateLimiterModel();
        var weatherRate = new WeatherRateLimiterModel();

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