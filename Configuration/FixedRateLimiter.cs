using Gridly.Models;
using Gridly.Services;
using Microsoft.AspNetCore.RateLimiting;
using DateTime = System.DateTime;

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
        IDataConverter<DateTime> dataConverter,
        FixedRateLimiterModel fixedRate) {
        
        services.AddRateLimiter( options =>
        {
            fixedRate.Window = GetFixedWindowData(fileService, dataConverter).Result; 
            options.AddFixedWindowLimiter(Policy, opt =>
            {
                opt.PermitLimit = fixedRate.Limit;
                opt.Window = fixedRate.Window;
                opt.QueueLimit = fixedRate.QueueLimit;
            });
        });

        return services;
    }
    
    private static async Task<TimeSpan> GetFixedWindowData(
        IFileService fileService, 
        IDataConverter<DateTime> dataConverter)
    {
        DateTime lastStoreRunTime;    
        if (!fileService.FileExcist(FilePaths.FixedRateLimitFilePath))
        {
            lastStoreRunTime = DateTime.Now;
            fileService.WriteToJson(dataConverter.SerializerToJsonString(lastStoreRunTime), 
                FilePaths.FixedRateLimitFilePath);
            return TimeSpan.FromHours(8);
        }
        
        var jsonString = await fileService.ReadAllFromFileAsync(FilePaths.FixedRateLimitFilePath);
        lastStoreRunTime = dataConverter.DeserializeJsonString(jsonString);
        
        var currentDateTime = DateTime.Now;
        var elapsedTime = DateTime.Now - lastStoreRunTime;
        if (elapsedTime >= TimeSpan.FromHours(8))
        {
            jsonString = dataConverter.SerializerToJsonString(currentDateTime);
            fileService.WriteToJson(FilePaths.FixedRateLimitFilePath, jsonString);
            return elapsedTime;
        }   
        
        return elapsedTime;
    }

    public static IApplicationBuilder UseFixedRateLimiter(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        app.UseMiddleware<FixedRateLimiterMiddleware>();
        return app;
    }
}