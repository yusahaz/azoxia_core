namespace Azoxia.Core.Application
{
    /// <summary>
    /// Dispatches an application request of type <typeparamref name="TRequest"/> and returns <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public interface IRequestHandler<in TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        #region Methods

        /// <summary>
        /// Executes the request asynchronously.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The handler result.</returns>
        Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
