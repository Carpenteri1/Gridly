using Gridly.Configuration;
using Gridly.Data;
using Gridly.EndPoints;

var appDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
Directory.SetCurrentDirectory(appDirectory);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = appDirectory
});

await builder.Services.AddTokenBucketRateLimiter();
builder.Services.Add();
builder.Services.AddScoped();
builder.Services.AddSingleton();

var app = builder.Build();
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<GridlyDbContext>().Database.EnsureCreated();
app.MapApiEndpoints();
app.UseStaticFiles();

app.MapDefaultControllerRoute().RequireRateLimiting("fixed");

app.UseTokenBucketRateLimiter();
app.MapFallbackToFile("index.html");

app.Run();
