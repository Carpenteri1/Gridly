namespace Gridly.Handlers;

public interface IVersionHandler<T>
{
    public Task<T> Handler();
}