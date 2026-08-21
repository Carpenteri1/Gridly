namespace Gridly.EndPoints.Interfaces;

public interface IProvidersEndPoint
{
    public Task<int> Validate(string rawKey);
}
