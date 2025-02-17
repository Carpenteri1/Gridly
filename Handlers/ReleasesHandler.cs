using System.Net;

namespace Gridly.Handler;

public class ReleasesHandler
{
    public static async Task<string> GetLatestVersion()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            //TODO fix forbidden error
            var response = await client.GetAsync("https://api.github.com/repos/Carpenteri1/Gridly/releases/latest");
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}