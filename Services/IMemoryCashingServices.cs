namespace Gridly.Services;

public interface IMemoryCashingService
{
    public T? Get<T>(string key) where T : class;
    public bool Store<T>(string key, T item, TimeSpan? absoluteExpiration = null) where T : class;
}