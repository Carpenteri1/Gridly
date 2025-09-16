namespace Gridly.Services;

public interface IHttpClientServices
{
    public Task<(bool, string)> Get(string Url);
}