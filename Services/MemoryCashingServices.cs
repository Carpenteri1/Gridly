using Microsoft.Extensions.Caching.Memory;

namespace Gridly.Services;

public class MemoryCashingServices(IMemoryCache memoryCache) : IMemoryCashingService
    
{
    public T Get<T>(string key) where T : class => memoryCache.Get<T>(key);
    public bool Store<T>(string key, T item, TimeSpan? absoluteExpiration = null) where T : class =>
        memoryCache.Set(
            key,
            item,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(60)
            }) is T;
}