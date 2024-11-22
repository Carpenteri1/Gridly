var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();  // Ensure static files are served

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}");

app.Run();