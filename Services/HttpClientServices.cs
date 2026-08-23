namespace Gridly.Services;

public class HttpClientServices() : IHttpClientServices
{
    public async Task<(bool, string)> Get(string Url)
    {
        var response = await SetupClient(Url);
        return
        (
            response.IsSuccessStatusCode,
            await response.Content.ReadAsStringAsync()
        );
    }

    public async Task<(int StatusCode, string Body)> GetWithStatusCode(string Url)
    {
        try
        {
            var response = await SetupClient(Url);
            var body = await response.Content.ReadAsStringAsync();
            return ((int)response.StatusCode, body);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return (0, string.Empty);
        }
    }

    private async Task<HttpResponseMessage> SetupClient(string url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Gridly");
        return await client.GetAsync(url);
    }
}