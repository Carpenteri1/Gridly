using Gridly.Constants;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.EndPoints;

public class VersionEndPoint(
    IDataConverter<VersionModel> dataConverter, 
    IHttpClientServices httpClientServices) : IVersionEndPoint
{
    public async Task<(bool, VersionModel?)> GetLatestVersion()
    {
        VersionModel version = null;
        var (success,item) = await httpClientServices.Get(EndpointStrings.GetVersionRemoteEndPoint);
        if (success) version = dataConverter.DeserializeJson(item);
        return (success, version);
    }

    public async Task<(bool, VersionModel?)> GetVersion()
    {
        VersionModel version = null;
       var (success,item) = await httpClientServices.Get(EndpointStrings.GetVersionInternalEndPoint);
       if (success) version = dataConverter.DeserializeJson(item);
       return (success, version);
    }
}