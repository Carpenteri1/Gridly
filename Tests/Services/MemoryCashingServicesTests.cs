using Gridly.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Gridly.Tests.Services;

public class MemoryCashingServicesTests
{
    [Fact]
    public void Store_ThenGet_ReturnsStoredInstance()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCashingServices(memoryCache);
        var version = "1.2.3";

        var stored = service.Store("version", version);
        var cached = service.Get<string>("version");

        Assert.True(stored);
        Assert.Equal(version, cached);
    }

    [Fact]
    public void Get_MissingKey_ReturnsNull()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCashingServices(memoryCache);

        var cached = service.Get<string>("missing");

        Assert.Null(cached);
    }
}
