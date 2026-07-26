using Gridly.Constants;
using Gridly.Models;
using Gridly.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Gridly.Configuration;

public static class RateLimiterPolicySettings
{
    public const string VersionPolicy = "version";
    public const string WeatherPolicy = "weather";

    public static async Task<IServiceCollection> AddTokenBucketRateLimiter(this IServiceCollection services) {

        var versionRate = new VersionRateLimiterModel();
        var weatherRate = new WeatherRateLimiterModel();

        services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, token) =>
            {
                if (!context.HttpContext.Request.Path.StartsWithSegments("/api/version"))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    return;
                }

                var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCashingService>();
                var state = cache.Get<VersionCheckStateModel>(CacheKeyStrings.VersionCacheKey);

                context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                if (state != null)
                    await context.HttpContext.Response.WriteAsJsonAsync(state.Version, token);
            };

            options
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
                });
        });
        return services;
    }

    public static IApplicationBuilder UseTokenBucketRateLimiter(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        app.UseMiddleware<TokenBucketRateLimiterMiddleware>();
        return app;
    }
}
