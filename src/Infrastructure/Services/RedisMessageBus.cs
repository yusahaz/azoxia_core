namespace Azoxia.Core.Infrastructure.Services
{
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Extensions;

    using Microsoft.Extensions.Logging;

    using StackExchange.Redis;

    /// <summary>
    /// <see cref="IRedisMessageBus"/> backed by StackExchange.Redis pub/sub.
    /// </summary>
    internal sealed class RedisMessageBus(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisMessageBus> logger) :
        IRedisMessageBus
    {
        #region Fields

        private readonly ILogger<RedisMessageBus> _logger = logger;

        private readonly ISubscriber _subscriber = connectionMultiplexer.GetSubscriber();

        #endregion Fields

        #region Methods

        /// <inheritdoc />
        public async Task PublishAsync(string channel, string payload, CancellationToken cancellationToken = default)
        {
            channel.ThrowIfNullOrWhiteSpace();
            payload.ThrowIfNull();

            try
            {
                await _subscriber.PublishAsync(RedisChannel.Literal(channel), payload).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, "Redis publish cancelled for channel {RedisChannel}.", channel);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis publish failed for channel {RedisChannel}.", channel);
                throw;
            }
        }

        /// <inheritdoc />
        public Task<IAsyncDisposable> SubscribeAsync(
            string channel,
            Func<string, string, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
        {
            channel.ThrowIfNullOrWhiteSpace();
            handler.ThrowIfNull();

            RedisChannel redisChannel = RedisChannel.Literal(channel);
            void OnMessage(RedisChannel channelIgnored, RedisValue message)
            {
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await handler(channel, message.ToString()!, cancellationToken).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException ex)
                        {
                            _logger.LogDebug(ex, "Redis pub/sub handler cancelled for channel {RedisChannel}.", channel);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Redis pub/sub handler failed for channel {RedisChannel}.", channel);
                        }
                    },
                    CancellationToken.None);
            }

            _subscriber.Subscribe(redisChannel, OnMessage, CommandFlags.None);

            return Task.FromResult<IAsyncDisposable>(new RedisChannelSubscription(_subscriber, redisChannel, _logger));
        }

        #endregion Methods

        #region Nested

        /// <summary>
        /// Unsubscribes a Redis channel when disposed.
        /// </summary>
        private sealed class RedisChannelSubscription(ISubscriber subscriber, RedisChannel channel, ILogger<RedisMessageBus> logger) :
            IAsyncDisposable
        {
            #region Methods

            /// <inheritdoc />
            public async ValueTask DisposeAsync()
            {
                try
                {
                    await subscriber.UnsubscribeAsync(channel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Redis unsubscribe failed for channel {RedisChannel}.", channel.ToString());
                }
            }

            #endregion Methods
        }

        #endregion Nested
    }
}
