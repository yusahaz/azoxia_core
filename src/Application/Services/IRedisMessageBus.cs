namespace Azoxia.Core.Application.Services
{
    /// <summary>
    /// Application-level Redis pub/sub abstraction (implemented in Infrastructure).
    /// </summary>
    public interface IRedisMessageBus
    {
        #region Methods

        /// <summary>
        /// Publishes a UTF-8 text payload to the channel.
        /// </summary>
        /// <param name="channel">Redis channel name.</param>
        /// <param name="payload">Message body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task PublishAsync(string channel, string payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes to a channel; dispose the returned object to unsubscribe.
        /// </summary>
        /// <param name="channel">Redis channel name.</param>
        /// <param name="handler">Invoked with channel name, payload, and cancellation token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Subscription handle.</returns>
        Task<IAsyncDisposable> SubscribeAsync(
            string channel,
            Func<string, string, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
