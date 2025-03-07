using Gridly.Models;
using Gridly.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace Gridly.Configuration;

public class FixedRateLimiterMiddleware
{
    private readonly RequestDelegate _next;

    public FixedRateLimiterMiddleware(RequestDelegate next)
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
    static readonly string Policy = "fixed"; 
    public static IServiceCollection AddFixedRateLimiter(
        this IServiceCollection services, 
        IFileService fileService,
        IDataConverter<FixedRateLimiterModel> dataConverter)
    {
        var savedTime = fileService.ReadAllFromFileAsync(FilePaths.RateLimitFilePath).Result;
        int limit = 1;
        int queueLimit = 1;
       // TimeSpan window =
            
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(Policy, opt =>
            {
                opt.PermitLimit = limit;
                opt.Window = TimeSpan.FromHours(3);
                opt.QueueLimit = queueLimit;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseFixedRateLimiter(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        app.UseMiddleware<FixedRateLimiterMiddleware>();
        return app;
    }
}