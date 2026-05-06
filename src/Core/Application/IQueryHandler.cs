namespace Azoxia.Core.Application
{
    /// <summary>
    /// Handles queries of type <typeparamref name="TQuery"/> and returns <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The read result type.</typeparam>
    public interface IQueryHandler<in TQuery, TResult> :
        IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}
