using Gridly.Models;
using Gridly.Services;

namespace Gridly.EndPoints;

public class VersionEndPoint(IDataConverter<VersionModel> dataConverter) : IVersionEndPoint
{
    public async Task<(bool, VersionModel?)> GetLatestVersion()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Gridly");
            var response = await client.GetAsync("https://api.github.com/repos/Carpenteri1/Gridly/releases/latest");
            return 
            (
                response.IsSuccessStatusCode, 
                dataConverter.DeserializeJsonString(await response.Content.ReadAsStringAsync())
            );
        }
    }
}