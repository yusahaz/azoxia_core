namespace Azoxia.Core.Application
{
    /// <summary>
    /// Pipeline behavior that wraps the next delegate (middleware-style around the handler).
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public interface IPipelineBehavior<in TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        #region Methods

        /// <summary>
        /// Invokes this behavior and optionally calls <paramref name="next"/> to continue the pipeline.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="next">The next stage (handler or inner behavior).</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The pipeline result.</returns>
        Task<TResult> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<TResult> next,
            CancellationToken cancellationToken);

        #endregion Methods
    }
}
