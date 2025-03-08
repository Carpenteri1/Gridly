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
    public static async Task<IServiceCollection> AddFixedRateLimiter(this IServiceCollection services) {
        
        var fileService = new FileService();
        var dataConverter = new DataConverter<DateTime>();
        var fixedRate = new FixedRateLimiterModel();
        
        fixedRate.Window = await GetFixedWindowData(fileService, dataConverter); 
        services.AddRateLimiter( _ => _
            .AddFixedWindowLimiter(Policy, opt =>
            {
                opt.PermitLimit = fixedRate.Limit;
                opt.Window = fixedRate.Window;
                opt.QueueLimit = fixedRate.QueueLimit;
            }));

        return services;
    }
    
    private static async Task<TimeSpan> GetFixedWindowData(
        IFileService fileService, 
        IDataConverter<DateTime> dataConverter)
    {
        
        DateTime lastStoreRunTime;    
        if (fileService.FileExcist(FilePaths.FixedRateLimitFilePath))
        {
            var jsonString = await fileService.ReadAllFromFileAsync(FilePaths.FixedRateLimitFilePath);
            if (string.IsNullOrEmpty(jsonString) || jsonString == "{}")
            {
                fileService.WriteToJson(FilePaths.FixedRateLimitFilePath, 
                    dataConverter.SerializerToJsonString(DateTime.Now));
                return TimeSpan.FromHours(8);
            }
            lastStoreRunTime = dataConverter.DeserializeJsonString(jsonString);
        }
        else
        {
            fileService.WriteToJson(FilePaths.FixedRateLimitFilePath, 
                dataConverter.SerializerToJsonString(DateTime.Now));
            return TimeSpan.FromHours(8);
        }
        
        var currentDateTime = DateTime.Now;
        var elapsedTime = lastStoreRunTime - currentDateTime;
        
        if (elapsedTime >= TimeSpan.FromHours(8))
        {
            fileService.WriteToJson(FilePaths.FixedRateLimitFilePath, 
                dataConverter.SerializerToJsonString(currentDateTime));
            return TimeSpan.FromHours(8);
        } 
        
        return TimeSpan.Zero;
    }

    public static IApplicationBuilder UseFixedRateLimiter(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        app.UseMiddleware<FixedRateLimiterMiddleware>();
        return app;
    }
}