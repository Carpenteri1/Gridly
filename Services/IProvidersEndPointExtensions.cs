namespace Gridly.helpers;

public interface IProvidersEndPointExtensions
{
    public Task<(int Status, string Body)> CallWeatherProvider(string address, string provider, string rawKey);
}
