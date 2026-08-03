namespace Gridly.EndPoints;

public interface IProvidersEndPoint
{
    public Task<int> Validate(string rawKey);
}
