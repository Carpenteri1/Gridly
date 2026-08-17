using Gridly.Commands;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.EndPoints;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Tests.Infrastructure;

internal sealed class FakeFileService : IFileService
{
    public bool UploadIconResult { get; set; } = true;
    public bool DeleteIconResult { get; set; } = true;
    public IconModel? UploadedIcon { get; private set; }
    public (string Name, string Type)? DeletedIcon { get; private set; }
    public IEnumerable<FileInfo> Icons { get; set; } = Array.Empty<FileInfo>();

    public bool FileExist(string filePath) => File.Exists(filePath);
    public bool DeletedFile(string filePath) => true;
    public bool WriteAllBitesToFile(string filePath, string content) => true;
    public bool WriteToFile(string filePath, string content) => true;
    public Task<string> ReadAllFromFileAsync(string filePath) => Task.FromResult(string.Empty);
    public IEnumerable<FileInfo> GetAllIcons() => Icons;

    public bool UploadIcon(IconModel iconData)
    {
        UploadedIcon = iconData;
        return UploadIconResult;
    }

    public bool DeleteIcon(string name, string type)
    {
        DeletedIcon = (name, type);
        return DeleteIconResult;
    }
}

internal sealed class FakeMemoryCashingService : IMemoryCashingService
{
    private readonly Dictionary<string, object> _cache = new();

    public int GetCallCount { get; private set; }
    public int StoreCallCount { get; private set; }
    public string? LastStoredKey { get; private set; }
    public object? LastStoredValue { get; private set; }

    public T? Get<T>(string key) where T : class
    {
        GetCallCount++;
        return _cache.TryGetValue(key, out var value) ? value as T : null;
    }

    public bool Store<T>(string key, T item) where T : class
    {
        StoreCallCount++;
        LastStoredKey = key;
        LastStoredValue = item;
        _cache[key] = item;
        return true;
    }

    public void Seed<T>(string key, T item) where T : class
    {
        _cache[key] = item;
    }
}

internal sealed class FakeVersionEndPoint : IVersionEndPoint
{
    public int GetVersionCallCount { get; private set; }
    public int GetLatestVersionCallCount { get; private set; }
    public (bool Success, VersionModel? Version) GetVersionResult { get; set; }
    public (bool Success, VersionModel? Version) GetLatestVersionResult { get; set; }

    public Task<(bool, VersionModel?)> GetLatestVersion()
    {
        GetLatestVersionCallCount++;
        return Task.FromResult((GetLatestVersionResult.Success, GetLatestVersionResult.Version));
    }

    public Task<(bool, VersionModel?)> GetVersion()
    {
        GetVersionCallCount++;
        return Task.FromResult((GetVersionResult.Success, GetVersionResult.Version));
    }
}

internal sealed class FakeHttpClientServices : IHttpClientServices
{
    public int CallCount { get; private set; }
    public string? LastUrl { get; private set; }
    public (bool Success, string Response) Response { get; set; }
    public (int StatusCode, string Body) StatusCodeResponse { get; set; }

    public Task<(bool, string)> Get(string url)
    {
        CallCount++;
        LastUrl = url;
        return Task.FromResult((Response.Success, Response.Response));
    }

    public Task<(int StatusCode, string Body)> GetWithStatusCode(string url)
    {
        CallCount++;
        LastUrl = url;
        return Task.FromResult((StatusCodeResponse.StatusCode, StatusCodeResponse.Body));
    }
}

internal sealed class FakeWeatherEndPoint : IWeatherEndPoint
{
    public int GetCallCount { get; private set; }
    public (int Status, WeatherDataDto? Weather) Result { get; set; }

    public Task<(int, WeatherDataDto? Weather)> Get(string location, string rawKey)
    {
        GetCallCount++;
        return Task.FromResult(Result);
    }
}

internal sealed class FakeWeatherRepository : IWeatherRepository
{
    private readonly Dictionary<string, WeatherDataModel> _byAddress = new();
    private int _nextId = 1;

    public int UpsertCallCount { get; private set; }
    public bool DeleteIfOrphanedResult { get; set; } = true;

    public void Seed(WeatherDataModel weather)
    {
        if (weather.Id == 0) weather.Id = _nextId;
        _nextId = Math.Max(_nextId, weather.Id + 1);
        _byAddress[weather.Address] = weather;
    }

    public Task<WeatherDataModel?> Get(string address) =>
        Task.FromResult(_byAddress.TryGetValue(address, out var value) ? value : null);

    public Task<IEnumerable<StoredWeatherDataDto>?> GetStoredWeatherData() =>
        Task.FromResult<IEnumerable<StoredWeatherDataDto>?>(Array.Empty<StoredWeatherDataDto>());

    public Task<WeatherDataModel> Upsert(WeatherDataModel weather)
    {
        UpsertCallCount++;
        weather.Id = _byAddress.TryGetValue(weather.Address, out var existing) ? existing.Id : _nextId++;
        _byAddress[weather.Address] = weather;
        return Task.FromResult(weather);
    }
}

internal sealed class FakeWeatherDataConnectionRepository : IWeatherDataConnectionRepository
{
    private readonly List<WeatherDataConnectionDtoModel> _connections = new();
    private readonly Dictionary<string, WeatherDataModel> _byAddress = new();
    private readonly Dictionary<int, WeatherDataModel> _byCardId = new();

    public IEnumerable<WeatherDataModel>? StoredWeatherData { get; set; }
    public int UpdateCallCount { get; private set; }
    public int InsertCallCount { get; private set; }
    public int UpsertCallCount { get; private set; }
    public int DeleteCallCount { get; private set; }
    public List<int> DeletedCardIds { get; } = new();
    
    public void Seed(int cardId, int weatherId) =>
        _connections.Add(new WeatherDataConnectionDtoModel { CardId = cardId, WeatherId = weatherId });

    public Task<WeatherDataModel> Get(string address) =>
        Task.FromResult(_byAddress.GetValueOrDefault(address)!);

    public Task<WeatherDataModel> GetById(int cardId) =>
        Task.FromResult(_byCardId.GetValueOrDefault(cardId)!);

    public Task<IEnumerable<WeatherDataModel>?> GetStoredWeatherData() =>
        Task.FromResult(StoredWeatherData);
    
    public Task<IEnumerable<WeatherDataConnectionDtoModel>> GetManyById(int? cardId, int? weatherId)
    {
        var results = _connections.Where(c =>
            (cardId is null || c.CardId == cardId) &&
            (weatherId is null || c.WeatherId == weatherId));
        return Task.FromResult<IEnumerable<WeatherDataConnectionDtoModel>>(results.ToList());
    }

    public Task<bool> Update(WeatherDataModel weather)
    {
        UpdateCallCount++;
        return Task.FromResult(true);
    }

    public Task<bool> Insert(WeatherDataModel weather)
    {
        InsertCallCount++;
        return Task.FromResult(true);
    }
    
    public Task<WeatherDataConnectionDtoModel> Upsert(WeatherDataConnectionDtoModel model)
    {
        UpsertCallCount++;
        _connections.RemoveAll(c => c.CardId == model.CardId);
        _connections.Add(model);
        return Task.FromResult(model);
    }
    
    public Task<bool> Delete(int cardId)
    {
        DeleteCallCount++;
        DeletedCardIds.Add(cardId);
        var removed = _connections.RemoveAll(c => c.CardId == cardId);
        return Task.FromResult(removed > 0);
    }
    
}

internal sealed class FakeMediator : IMediator
{
    public List<object> SentRequests { get; } = new();
    public Func<GetVisualCrossingDataQuery, IResult>? OnGetVisualCrossingData { get; set; }
    public Func<SaveWeatherCommand, IResult>? OnSaveWeather { get; set; }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        SentRequests.Add(request);

        object? response = request switch
        {
            GetVisualCrossingDataQuery query => OnGetVisualCrossingData?.Invoke(query) ?? Results.NotFound(),
            SaveWeatherCommand command => OnSaveWeather?.Invoke(command) ?? Results.Ok(),
            _ => throw new NotSupportedException($"FakeMediator does not handle {request.GetType()}")
        };

        return Task.FromResult((TResponse)response!);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest =>
        throw new NotSupportedException();

    public Task<object?> Send(object request, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
        throw new NotSupportedException();
}
internal sealed class FakeLocalProvidersRepository : ILocalProvidersRepository
{
    public ProviderKeyDtoModel? StoredKey { get; set; }
    public int UpsertCallCount { get; private set; }
    public int UpdateStatusCallCount { get; private set; }
    public string? LastUpdatedStatus { get; private set; }

    public Task<ProviderKeyDtoModel?> Get(string provider) => Task.FromResult(StoredKey);

    public Task<bool> Upsert(string provider, string encryptedKey, string status)
    {
        UpsertCallCount++;
        StoredKey = new ProviderKeyDtoModel
        {
            Provider = provider,
            EncryptedKey = encryptedKey,
            Status = status,
            LastValidatedAt = DateTime.UtcNow
        };
        return Task.FromResult(true);
    }

    public Task<bool> UpdateStatus(string provider, string status)
    {
        UpdateStatusCallCount++;
        LastUpdatedStatus = status;
        if (StoredKey is not null) StoredKey.Status = status;
        return Task.FromResult(true);
    }
}

internal sealed class FakeProviderKeysProtectionService : IProviderKeysProtectionService
{
    public string Protect(string rawKey) => $"protected:{rawKey}";
    public string Unprotect(string encryptedKey) => encryptedKey.Replace("protected:", "");
}

internal sealed class FakeProvidersEndPoint : IProvidersEndPoint
{
    public int ValidateCallCount { get; private set; }
    public int Result { get; set; } = StatusCodes.Status200OK;

    public Task<int> Validate(string provider)
    {
        ValidateCallCount++;
        return Task.FromResult(Result);
    }
}

internal sealed class FakeGridlyDbContext : IGridlyDbContext
{
    public List<CardEntity> CardEntities { get; } = [];
    public List<SettingsEntity> SettingsEntities { get; } = [];
    public List<IconsConnectedEntity> IconsConnectedEntities { get; } = [];
    public List<IconEntity> IconEntities { get; } = [];

    public IQueryable<CardEntity> Cards => CardEntities.AsQueryable();
    public IQueryable<SettingsEntity> Settings => SettingsEntities.AsQueryable();
    public IQueryable<IconsConnectedEntity> IconsConnected => IconsConnectedEntities.AsQueryable();
    public IQueryable<IconEntity> Icons => IconEntities.AsQueryable();
}
