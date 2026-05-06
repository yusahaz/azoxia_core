namespace Azoxia.Core.Application.Queries
{
    /// <summary>
    /// Base class for queries returning <typeparamref name="TResult"/> when handled.
    /// </summary>
    /// <typeparam name="TResult">The result type produced by the query handler.</typeparam>
    public abstract class QueryBase<TResult> :
        IQuery<TResult>
    {
    }
}
