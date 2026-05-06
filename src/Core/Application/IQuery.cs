namespace Azoxia.Core.Application
{
    /// <summary>
    /// Marker for a read-side request that returns <typeparamref name="TResult"/> (query in CQRS terms).
    /// </summary>
    /// <typeparam name="TResult">The read model or DTO type returned by the handler.</typeparam>
    public interface IQuery<TResult> :
        IRequest<TResult>
    {
    }
}
