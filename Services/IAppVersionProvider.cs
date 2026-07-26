namespace Gridly.Services;

public interface IAppVersionProvider
{
    Task<string> GetCurrentVersionAsync();
}
