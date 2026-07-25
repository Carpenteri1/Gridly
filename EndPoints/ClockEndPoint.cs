using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.EndPoints;

public class ClockEndPoint(
    IDataConverter<ClockDtoModel> dataConverter,
    IHttpClientServices httpClientServices) : IClockEndPoint
{
    public async Task<(ClockFetchStatus Status, ClockModel? Clock)> Get(string timeZone)
    {
        var url = string.Format(EndpointStrings.GetTimeApiCurrentZone, Uri.EscapeDataString(timeZone));

        var (statusCode, body) = await httpClientServices.GetWithStatusCode(url);

        if (statusCode is < 200 or >= 300) return (ClockFetchStatus.ProviderUnavailable, null);

        var dto = dataConverter.DeserializeJson(body);
        return dto is null
            ? (ClockFetchStatus.ProviderUnavailable, null)
            : (ClockFetchStatus.Success, ClockFactory.Create(dto));
    }
}
