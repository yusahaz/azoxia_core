namespace Azoxia.Core.Application
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Resolves <see cref="IRequestHandler{TRequest, TResult}"/> and composes <see cref="IPipelineBehavior{TRequest, TResult}"/> instances into a delegate chain.
    /// </summary>
    /// <typeparam name="TRequest">The concrete request type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal sealed class RequestHandlerWrapper<TRequest, TResult> :
        RequestHandlerWrapperBase<TResult>
        where TRequest : IRequest<TResult>
    {
        #region Methods

        /// <inheritdoc />
        public override Task<TResult> HandleAsync(
            IRequest<TResult> request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            Task<TResult> Handler() => serviceProvider
                .GetRequiredService<IRequestHandler<TRequest, TResult>>()
                .HandleAsync((TRequest)request, cancellationToken);

            return serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResult>>()
                .Reverse()
                .Aggregate(
                    (RequestHandlerDelegate<TResult>)Handler,
                    (next, behavior) => () => behavior.HandleAsync((TRequest)request, next, cancellationToken))();
        }

        #endregion Methods
    }
}
