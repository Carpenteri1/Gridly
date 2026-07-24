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