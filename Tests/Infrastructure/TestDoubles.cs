using Gridly.Commands;
using Gridly.Dtos;
using Gridly.EndPoints.Interfaces;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Services;
using MediatR;

namespace Gridly.Tests.Infrastructure;

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
