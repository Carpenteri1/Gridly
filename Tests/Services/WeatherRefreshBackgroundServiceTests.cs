using Gridly.Constants;
using Gridly.Dtos;
using Gridly.EndPoints;
using Gridly.Repositories;
using Gridly.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gridly.Tests.Services;

public class WeatherRefreshBackgroundServiceTests
{
    private static TestableWeatherPeriodicRefreshBackgroundService BuildService(
        RecordingWeatherRepository repository,
        RecordingWeatherEndPoint? weatherEndPoint = null,
        FakeLocalProvidersRepository? localProvidersRepository = null,
        FakeProviderKeysProtectionService? providerKeysProtectionService = null)
    {
        localProvidersRepository ??= new FakeLocalProvidersRepository();
        localProvidersRepository.StoredKey ??= new ProviderKeyDtoModel
        {
            Provider = EndpointStrings.VisualCrossingProvider,
            EncryptedKey = "protected:test-key",
            Status = "Valid"
        };

        return new TestableWeatherPeriodicRefreshBackgroundService(
            NullLogger<WeatherPeriodicRefreshBackgroundService>.Instance,
            repository,
            weatherEndPoint ?? new RecordingWeatherEndPoint(),
            localProvidersRepository,
            providerKeysProtectionService ?? new FakeProviderKeysProtectionService());
    }

    [Fact]
    public async Task RefreshAll_WithNoStoredWeather_DoesNotCallProvider()
    {
        var repository = new RecordingWeatherRepository();
        var weatherEndPoint = new RecordingWeatherEndPoint();
        var service = BuildService(repository, weatherEndPoint);

        await service.RefreshNow(CancellationToken.None);

        Assert.Equal(0, weatherEndPoint.GetCallCount);
        Assert.Empty(repository.InsertedWeather);
    }

    [Fact]
    public async Task RefreshAll_ForStoredAddress_FetchesFreshDataAndSavesIt()
    {
        var stored = new WeatherDataModel { CardId = 7, Address = "Stockholm" };
        var repository = new RecordingWeatherRepository { StoredWeatherData = [stored] };
        var weatherEndPoint = new RecordingWeatherEndPoint
        {
            Result = (StatusCodes.Status200OK, CreateWeatherDto("Stockholm", "clear"))
        };
        var service = BuildService(repository, weatherEndPoint);

        await service.RefreshNow(CancellationToken.None);

        var savedWeather = Assert.Single(repository.InsertedWeather);
        Assert.Equal("Stockholm", savedWeather.Address);
        Assert.Equal("clear", savedWeather.Description);
        Assert.Equal("Stockholm", weatherEndPoint.Requests.Single().Location);
        Assert.Equal("test-key", weatherEndPoint.Requests.Single().RawKey);
    }

    [Fact]
    public async Task RefreshAll_WhenProviderCallIsNotOk_DoesNotSave()
    {
        var stored = new WeatherDataModel { CardId = 1, Address = "Stockholm" };
        var repository = new RecordingWeatherRepository { StoredWeatherData = [stored] };
        var weatherEndPoint = new RecordingWeatherEndPoint
        {
            Result = (StatusCodes.Status401Unauthorized, null)
        };
        var service = BuildService(repository, weatherEndPoint);

        await service.RefreshNow(CancellationToken.None);

        Assert.Empty(repository.InsertedWeather);
    }

    [Fact]
    public async Task RefreshAll_WhenOneAddressThrows_StillProcessesTheRest()
    {
        var first = new WeatherDataModel { CardId = 1, Address = "Stockholm" };
        var second = new WeatherDataModel { CardId = 2, Address = "Oslo" };
        var repository = new RecordingWeatherRepository { StoredWeatherData = [first, second] };
        var weatherEndPoint = new RecordingWeatherEndPoint
        {
            OnGet = location => location == "Stockholm"
                ? throw new InvalidOperationException("provider unavailable")
                : (StatusCodes.Status200OK, CreateWeatherDto(location))
        };
        var service = BuildService(repository, weatherEndPoint);
        service.DelayHandler = (_, _) => Task.CompletedTask;

        await service.RefreshNow(CancellationToken.None);

        var savedWeather = Assert.Single(repository.InsertedWeather);
        Assert.Equal("Oslo", savedWeather.Address);
    }

    [Fact]
    public async Task RefreshAll_WithMultipleAddresses_DelaysBetweenCallsButNotBeforeTheFirst()
    {
        WeatherDataModel[] entries =
        [
            new() { CardId = 1, Address = "Stockholm" },
            new() { CardId = 2, Address = "Oslo" },
            new() { CardId = 3, Address = "Helsinki" }
        ];
        var repository = new RecordingWeatherRepository { StoredWeatherData = entries };
        var weatherEndPoint = new RecordingWeatherEndPoint
        {
            OnGet = location => (StatusCodes.Status200OK, CreateWeatherDto(location))
        };
        var service = BuildService(repository, weatherEndPoint);
        var delays = new List<TimeSpan>();
        service.DelayHandler = (delay, _) =>
        {
            delays.Add(delay);
            return Task.CompletedTask;
        };

        await service.RefreshNow(CancellationToken.None);

        Assert.Equal(2, delays.Count);
        Assert.All(delays, d => Assert.Equal(service.ExpectedDelayBetweenProviderCalls, d));
    }

    private static WeatherDataDto CreateWeatherDto(string address, string description = "clear") =>
        new(
            address,
            "Europe/Stockholm",
            description,
            [new DaysDto(22, 21, 55, 6, 180)],
            DateTime.UtcNow);

    private sealed class TestableWeatherPeriodicRefreshBackgroundService(
        ILogger<WeatherPeriodicRefreshBackgroundService> logger,
        IWeatherRepository weatherRepository,
        IWeatherEndPoint weatherEndPoint,
        ILocalProvidersRepository localProvidersRepository,
        IProviderKeysProtectionService providerKeysProtectionService)
        : WeatherPeriodicRefreshBackgroundService(
            logger,
            weatherRepository,
            weatherEndPoint,
            localProvidersRepository,
            providerKeysProtectionService)
    {
        public TimeSpan ExpectedDelayBetweenProviderCalls => DelayBetweenProviderCalls;

        public Func<TimeSpan, CancellationToken, Task> DelayHandler
        {
            set => Delay = value;
        }

        public Task RefreshNow(CancellationToken cancellationToken) => RefreshData(cancellationToken);
    }

    private sealed class RecordingWeatherRepository : IWeatherRepository
    {
        public IEnumerable<WeatherDataModel>? StoredWeatherData { get; set; }
        public List<WeatherDataModel> InsertedWeather { get; } = [];

        public Task<WeatherDataModel> Get(string address) => throw new NotSupportedException();

        public Task<WeatherDataModel> GetById(int cardId) => throw new NotSupportedException();

        public Task<IEnumerable<WeatherDataModel>?> GetStoredWeatherData() =>
            Task.FromResult(StoredWeatherData);

        public Task<bool> Update(WeatherDataModel weather) => throw new NotSupportedException();

        public Task<bool> Insert(WeatherDataModel weather)
        {
            InsertedWeather.Add(weather);
            return Task.FromResult(true);
        }
    }

    private sealed class RecordingWeatherEndPoint : IWeatherEndPoint
    {
        public int GetCallCount { get; private set; }
        public List<(string Location, string RawKey)> Requests { get; } = [];
        public (int Status, WeatherDataDto? Weather) Result { get; set; } = (StatusCodes.Status404NotFound, null);
        public Func<string, (int Status, WeatherDataDto? Weather)>? OnGet { get; set; }

        public Task<(int, WeatherDataDto? Weather)> Get(string address, string rawKey)
        {
            GetCallCount++;
            Requests.Add((address, rawKey));
            return Task.FromResult(OnGet?.Invoke(address) ?? Result);
        }
    }

    private sealed class FakeLocalProvidersRepository : ILocalProvidersRepository
    {
        public ProviderKeyDtoModel? StoredKey { get; set; }

        public Task<ProviderKeyDtoModel?> Get(string provider) => Task.FromResult(StoredKey);

        public Task<bool> Upsert(string provider, string encryptedKey, string status) =>
            throw new NotSupportedException();

        public Task<bool> UpdateStatus(string provider, string status) =>
            throw new NotSupportedException();
    }

    private sealed class FakeProviderKeysProtectionService : IProviderKeysProtectionService
    {
        public string Protect(string rawKey) => $"protected:{rawKey}";

        public string Unprotect(string encryptedKey) => encryptedKey.Replace("protected:", "");
    }
}
