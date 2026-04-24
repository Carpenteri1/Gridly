using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;

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

    public Task<(bool, string)> Get(string url)
    {
        CallCount++;
        LastUrl = url;
        return Task.FromResult((Response.Success, Response.Response));
    }
}
