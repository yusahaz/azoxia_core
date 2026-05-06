namespace Azoxia.Core.Application
{
    using System;
    using System.Collections.Concurrent;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Dispatches <see cref="IRequest{TResult}"/> instances through cached handler wrappers and pipeline behaviors.
    /// </summary>
    internal class Sender(IServiceProvider serviceProvider) :
        ISender
    {
        #region Fields

        private static readonly ConcurrentDictionary<Type, object> _wrapperCache = new();

        #endregion Fields

        #region Methods

        /// <inheritdoc />
        public Task<TResult> SendAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            request.ThrowIfNull();

            var requestType = request.GetType();
            var wrapper = (RequestHandlerWrapperBase<TResult>)_wrapperCache.GetOrAdd(requestType, t =>
            {
                var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(t, typeof(TResult));
                var instance = Activator.CreateInstance(wrapperType);

                (instance is not null).ThrowIfFalse(AzoxiaErrorCodes.InvalidOperation);

                return instance;
            });

            return wrapper.HandleAsync(request, serviceProvider, cancellationToken);
        }

        #endregion Methods
    }
}
