namespace Azoxia.Core.Application
{
    using System;

    /// <summary>
    /// Non-generic base for per-request-type wrappers that resolve handlers from <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type returned by the pipeline.</typeparam>
    internal abstract class RequestHandlerWrapperBase<TResult>
    {
        #region Methods

        /// <summary>
        /// Resolves the handler and runs the request through behaviors (implemented by concrete wrappers).
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="serviceProvider">The service provider used to resolve handlers and behaviors.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The handler result.</returns>
        public abstract Task<TResult> HandleAsync(
            IRequest<TResult> request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);

        #endregion Methods
    }
}
