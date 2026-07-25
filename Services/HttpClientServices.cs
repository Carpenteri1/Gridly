namespace Gridly.Services;

public class HttpClientServices : IHttpClientServices
{
    public async Task<(bool, string)> Get(string Url)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Gridly");
            var response = await client.GetAsync(Url);
            return
            (
                response.IsSuccessStatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }
    }

    public async Task<(int StatusCode, string Body)> GetWithStatusCode(string Url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Gridly");
        try
        {
            var response = await client.GetAsync(Url);
            var body = await response.Content.ReadAsStringAsync();
            return ((int)response.StatusCode, body);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return (0, string.Empty);
        }
    }
}