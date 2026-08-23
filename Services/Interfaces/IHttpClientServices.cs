namespace Gridly.Services;

public interface IHttpClientServices
{
    public Task<(bool, string)> Get(string Url);
    public Task<(int StatusCode, string Body)> GetWithStatusCode(string Url);
}