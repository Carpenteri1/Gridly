using Gridly.Dtos;
using Gridly.EndPoints;
using Gridly.Enums;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gridly.Tests.Services;

public class WeatherPeriodicRefreshBackgroundServiceTests
{
    private static StoredWeatherDataDto MakeStoredEntry(string address = "London") =>
        new() { Address = address, Timezone = "Europe/London", Description = "", FetchedAt = DateTime.UtcNow };

    [Fact]
    public async Task RefreshData_WhenNoStoredWeatherData_ReturnsEarlyWithoutCallingEndpoint()
    {
        var weatherEndPoint = new FakeWeatherEndPoint();
        var weatherRepository = new FakeWeatherRepository { StoredWeatherData = null };
        var localProvidersRepository = new FakeLocalProvidersRepository();
        var service = CreateService(weatherRepository, weatherEndPoint, localProvidersRepository);

        await service.InvokeRefreshData(CancellationToken.None);

        Assert.Equal(0, weatherEndPoint.GetCallCount);
    }

    [Fact]
    public async Task RefreshData_WhenNoProviderKeyStored_ReturnsWithoutCallingEndpoint()
    {
        var weatherEndPoint = new FakeWeatherEndPoint();
        var weatherRepository = new FakeWeatherRepository { StoredWeatherData = [MakeStoredEntry()] };
        var localProvidersRepository = new FakeLocalProvidersRepository { StoredKey = null };
        var service = CreateService(weatherRepository, weatherEndPoint, localProvidersRepository);

        await service.InvokeRefreshData(CancellationToken.None);

        Assert.Equal(0, weatherEndPoint.GetCallCount);
    }

    [Fact]
    public async Task RefreshData_WhenEndpointReturnsFreshData_UpsertsAndMarksKeyValid()
    {
        var days = new[] { new DaysDto(temp: 20, feelslike: 19, humidity: 50, windspeed: 4, windDir: 90) };
        var weatherDto = new WeatherDataDto("London", "Europe/London", "Sunny", days, DateTime.UtcNow);
        var weatherEndPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status200OK, weatherDto) };
        var weatherRepository = new FakeWeatherRepository { StoredWeatherData = [MakeStoredEntry()] };
        var localProvidersRepository = new FakeLocalProvidersRepository
        {
            StoredKey = new ProviderKeyDtoModel
            {
                Provider = "VisualCrossing",
                EncryptedKey = "raw-key",
                Status = nameof(ProvidersKeyStatusEnum.Unknown)
            }
        };
        var service = CreateService(weatherRepository, weatherEndPoint, localProvidersRepository);

        await service.InvokeRefreshData(CancellationToken.None);

        Assert.Equal(1, weatherRepository.UpsertCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Valid), localProvidersRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task RefreshData_WhenEndpointThrowsHttpRequestException_MarksKeyInvalidWithoutUpserting()
    {
        var weatherEndPoint = new ThrowingWeatherEndPoint(new HttpRequestException("boom"));
        var weatherRepository = new FakeWeatherRepository { StoredWeatherData = [MakeStoredEntry()] };
        var localProvidersRepository = new FakeLocalProvidersRepository
        {
            StoredKey = new ProviderKeyDtoModel
            {
                Provider = "VisualCrossing",
                EncryptedKey = "raw-key",
                Status = nameof(ProvidersKeyStatusEnum.Valid)
            }
        };
        var service = CreateService(weatherRepository, weatherEndPoint, localProvidersRepository);

        await service.InvokeRefreshData(CancellationToken.None);

        Assert.Equal(0, weatherRepository.UpsertCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Invalid), localProvidersRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task RefreshData_WhenEndpointThrowsUnexpectedException_SwallowsAndContinues()
    {
        var weatherEndPoint = new ThrowingWeatherEndPoint(new InvalidOperationException("boom"));
        var weatherRepository = new FakeWeatherRepository { StoredWeatherData = [MakeStoredEntry()] };
        var localProvidersRepository = new FakeLocalProvidersRepository
        {
            StoredKey = new ProviderKeyDtoModel
            {
                Provider = "VisualCrossing",
                EncryptedKey = "raw-key",
                Status = nameof(ProvidersKeyStatusEnum.Valid)
            }
        };
        var service = CreateService(weatherRepository, weatherEndPoint, localProvidersRepository);

        await service.InvokeRefreshData(CancellationToken.None);

        Assert.Equal(0, weatherRepository.UpsertCallCount);
        Assert.Equal(0, localProvidersRepository.UpdateStatusCallCount);
    }

    private static TestableWeatherPeriodicRefreshBackgroundService CreateService(
        IWeatherRepository weatherRepository,
        IWeatherEndPoint weatherEndPoint,
        ILocalProvidersRepository localProvidersRepository)
    {
        var services = new ServiceCollection();
        services.AddSingleton(weatherRepository);
        services.AddSingleton(weatherEndPoint);
        services.AddSingleton(localProvidersRepository);
        var provider = services.BuildServiceProvider();

        var service = new TestableWeatherPeriodicRefreshBackgroundService(
            NullLogger<WeatherPeriodicRefreshBackgroundService>.Instance,
            provider.GetRequiredService<IServiceScopeFactory>(),
            new FakeProviderKeysProtectionService());
        service.SetDelay((_, _) => Task.CompletedTask);
        return service;
    }

    private sealed class TestableWeatherPeriodicRefreshBackgroundService(
        ILogger<WeatherPeriodicRefreshBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IProviderKeysProtectionService providerKeysProtectionService)
        : WeatherPeriodicRefreshBackgroundService(logger, serviceScopeFactory, providerKeysProtectionService)
    {
        public Task InvokeRefreshData(CancellationToken cancellationToken) => RefreshData(cancellationToken);
        public void SetDelay(Func<TimeSpan, CancellationToken, Task> delay) => Delay = delay;
    }

    private sealed class ThrowingWeatherEndPoint(Exception exception) : IWeatherEndPoint
    {
        public Task<(int, WeatherDataDto?)> Get(string address, string rawKey) => throw exception;
    }
}
