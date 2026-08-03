namespace Gridly.helpers;

public interface IProvidersEndPointExtensions
{
    public Task<(int Status, string Body)> CallWeatherProvider(string location, string provider, string rawKey);
}
