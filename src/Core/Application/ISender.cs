namespace Azoxia.Core.Application
{
    /// <summary>
    /// Entry point for dispatching application requests to their handlers (CQRS mediator-style).
    /// </summary>
    public interface ISender
    {
        #region Methods

        /// <summary>
        /// Sends the request through the handler pipeline and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The handler result.</returns>
        Task<TResult> SendAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
