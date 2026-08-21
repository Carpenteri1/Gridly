using Gridly.Configuration;
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

app.MapApiEndpoints();
app.UseStaticFiles();

app.MapDefaultControllerRoute().RequireRateLimiting("fixed");

//app.UseRouting();
app.UseTokenBucketRateLimiter();
app.MapFallbackToFile("index.html");

app.Run();
