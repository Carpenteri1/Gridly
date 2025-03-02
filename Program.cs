using Gridly.EndPoints;
using Gridly.Repositories;
using Gridly.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddSingleton<IVersionEndPoint, VersionEndPoint>();

builder.Services.AddSingleton<IComponentRepository,ComponentRepository>();
builder.Services.AddSingleton<IVersionRepository, VersionRepository>();

builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton(typeof(IDataConverter<>), typeof(DataConverter<>));

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

/* TODO
var rateLimit = new FixedRateLimitOptionsModel();
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: rateLimit.Policy, options =>
    {
        options.PermitLimit = rateLimit.Limit;
        options.Window = rateLimit.Window;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = rateLimit.QueueLimit;
    }));*/

var app = builder.Build();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Icons")),
    RequestPath = "/Assets/Icons"
});

app.MapDefaultControllerRoute().RequireRateLimiting("fixed");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}");

app.UseRouting();
//app.UseRateLimiter();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("index.html");
});

app.Run();