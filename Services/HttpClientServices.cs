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
}