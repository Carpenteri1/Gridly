using Gridly.Data;
using Gridly.EndPoints;
using Gridly.EndPoints.Interfaces;
using Gridly.helpers;
using Gridly.Repositories;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Configuration;

public static class ServiceCollection
{
    public static void AddScoped(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionServices,DbConnectionServices>();
        services.AddScoped<IVersionEndPoint, VersionEndPoint>();
        services.AddScoped<IWeatherEndPoint, WeatherEndPoint>();
        services.AddScoped<IProvidersEndPoint, ProvidersEndPoint>();
        services.AddScoped<ICardRepository,CardRepository>();
        services.AddScoped<IColumnRowRepository,ColumnRowRepository>();
        services.AddScoped<ISettingsRepository,SettingsRepository>();
        services.AddScoped<IIconRepository,IconRepository>();
        services.AddScoped<IIconConnectedRepository,IconConnectedRepository>();
        services.AddScoped<IWidgetRepository,WidgetRepository>();
        services.AddScoped<ILocalProvidersRepository,LocalProversRepository>();
        services.AddScoped<IWeatherRepository,WeatherRepository>();
        services.AddScoped<IWeatherDataConnectionRepository,WeatherDataConnectionRepository>();
        services.AddScoped<IProvidersEndPointExtensions,ProvidersEndPointExtensions>();
    }

    public static void AddSingleton(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryCashingService, MemoryCashingServices>();
        services.AddSingleton<IHttpClientServices, HttpClientServices>();
        services.AddSingleton<IProviderKeysProtectionService, ProviderKeysProtectionService>();
        services.AddSingleton(typeof(IDataConverter<>), typeof(DataConverter<>));
    }
    
    public static void Add(this IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddDbContext<GridlyDbContext>((sp, options) =>
            options.UseSqlite(sp.GetRequiredService<IDbConnectionServices>().GetConnectionString()));

        services.AddHostedService<WeatherPeriodicRefreshBackgroundService>();

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
    }

}