using Gridly.Configuration;
using Gridly.EndPoints;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Data;
using System.Data;

var appDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
Directory.SetCurrentDirectory(appDirectory);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = appDirectory
});

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
await builder.Services.AddTokenBucketRateLimiter();

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped(sp =>
      sp.GetRequiredService<IDbConnectionServices>().CreateConnection());
builder.Services.AddScoped<IVersionEndPoint, VersionEndPoint>();
builder.Services.AddScoped<ICardRepository,CardRepository>();
builder.Services.AddScoped<ISettingsRepository,SettingsRepository>();
builder.Services.AddScoped<IIconRepository,IconRepository>();
builder.Services.AddScoped<IIconConnectedRepository,IconConnectedRepository>();

builder.Services.AddSingleton<IDbConnectionServices,DbConnectionServices>();
builder.Services.AddSingleton<IMemoryCashingService, MemoryCashingServices>();
builder.Services.AddSingleton<IHttpClientServices, HttpClientServices>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton(typeof(IDataConverter<>), typeof(DataConverter<>));

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
var app = builder.Build();

app.UseStaticFiles();

app.MapDefaultControllerRoute().RequireRateLimiting("fixed");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}");

app.UseRouting();
app.UseTokenBucketRateLimiter();

using (var scope = app.Services.CreateScope())
{
    var dbInit = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInit.EnsureTablesCreatedAsync();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("index.html");
});

app.Run();
