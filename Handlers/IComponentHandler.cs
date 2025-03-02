namespace Gridly.Handlers;

public interface IComponentHandler<T>
{
    public IResult Handler(T command);
}