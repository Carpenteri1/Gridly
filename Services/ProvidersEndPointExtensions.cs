using Gridly.Services;

namespace Gridly.helpers;

public class ProvidersEndPointExtensions(IHttpClientServices httpClientServices) : IProvidersEndPointExtensions
{
    public async Task<(int Status, string Body)> CallWeatherProvider(string location, string provider, string rawKey)
    {
        var url = string.Format(
            provider,
            Uri.EscapeDataString(location),
            rawKey);
        
        var (statusCode, body) = await httpClientServices.GetWithStatusCode(url);
        return (statusCode, body);
    }
}