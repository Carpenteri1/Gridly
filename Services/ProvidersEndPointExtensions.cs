using Gridly.Services;

namespace Gridly.helpers;

public class ProvidersEndPointExtensions(IHttpClientServices httpClientServices) : IProvidersEndPointExtensions
{
    public async Task<(int Status, string Body)> CallWeatherProvider(string address, string provider, string rawKey)
    {
        var url = string.Format(
            provider,
            Uri.EscapeDataString(address),
            rawKey);
        
        var (statusCode, body) = await httpClientServices.GetWithStatusCode(url);
        return (statusCode, body);
    }
}