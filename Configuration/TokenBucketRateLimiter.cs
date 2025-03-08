using Gridly.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Configuration;

public class TokenBucketRateLimiterMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBucketRateLimiterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
    }
}

public static class RequestRateLimiterExtensions 
{
    static readonly string Policy = "TokenLimiter"; 
    public static async Task<IServiceCollection> AddTokenBucketRateLimiter(this IServiceCollection services) {
        
        var fixedRate = new FixedRateLimiterModel();
        
        services.AddRateLimiter(_ => _
            .AddTokenBucketLimiter(Policy, opt =>
            {
                opt.TokenLimit = fixedRate.Limit;
                opt.QueueLimit = fixedRate.QueueLimit;
                opt.ReplenishmentPeriod = fixedRate.Window; 
                opt.TokensPerPeriod = fixedRate.TokensPerPeriod;
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