using Gridly.Configuration;
using Gridly.EndPoints;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
await builder.Services.AddTokenBucketRateLimiter();

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<System.Data.IDbConnection>(sp =>
    new SqliteConnection(builder.Configuration.GetConnectionString("GridlyDb")));
builder.Services.AddScoped<IVersionEndPoint, VersionEndPoint>();
builder.Services.AddScoped<IVersionRepository, VersionRepository>();
builder.Services.AddScoped<IComponentRepository,ComponentRepository>();
builder.Services.AddScoped<IComponentSettingsRepository,ComponentSettingsRepository>();
builder.Services.AddScoped<IIconRepository,IconRepository>();
builder.Services.AddScoped<IIconConnectedRepository,IconConnectedRepository>();

builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton(typeof(IDataConverter<>), typeof(DataConverter<>));

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
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