namespace Azoxia.Core.Infrastructure.Services
{
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// No-op <see cref="IRedisMessageBus"/> when Redis is disabled so DI always resolves an implementation.
    /// </summary>
    internal sealed class NullRedisMessageBus :
        IRedisMessageBus
    {
        #region Methods

        /// <inheritdoc />
        public Task PublishAsync(string channel, string payload, CancellationToken cancellationToken = default)
        {
            channel.ThrowIfNullOrWhiteSpace();
            payload.ThrowIfNull();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IAsyncDisposable> SubscribeAsync(
            string channel,
            Func<string, string, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
        {
            channel.ThrowIfNullOrWhiteSpace();
            handler.ThrowIfNull();
            return Task.FromResult<IAsyncDisposable>(EmptySubscription.Instance);
        }

        #endregion Methods

        #region Nested

        /// <summary>
        /// Trivial async disposable for no-op subscriptions.
        /// </summary>
        private sealed class EmptySubscription :
            IAsyncDisposable
        {
            #region Fields

            internal static readonly EmptySubscription Instance = new();

            #endregion Fields

            #region Methods

            /// <inheritdoc />
            public ValueTask DisposeAsync()
                => ValueTask.CompletedTask;

            #endregion Methods
        }

        #endregion Nested
    }
}
