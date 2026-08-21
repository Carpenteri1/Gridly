using Gridly.Constants;
using Gridly.EndPoints.Interfaces;
using Gridly.helpers;

namespace Gridly.EndPoints;

public class ProvidersEndPoint(IProvidersEndPointExtensions providersEndPointExtensions) : IProvidersEndPoint 
{
    public async Task<int> Validate(string rawKey)
    {
        var (statusCode, _) = await providersEndPointExtensions.CallWeatherProvider(
            EndpointStrings.VisualCrossingProbeLocation,
            EndpointStrings.GetVisualCrossingWeatherData, 
            rawKey);
        
        return statusCode;
    }
}
