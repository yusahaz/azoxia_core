namespace Azoxia.Core.Application.Commands
{
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Persistence;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Base class for command handlers that return <see cref="Unit"/>; resolves <see cref="IUnitOfWork"/> and logging from <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command type handled by this handler.</typeparam>
    public abstract class CommandHandlerBase<TCommand> :
        ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase{TCommand}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to resolve infrastructure dependencies.</param>
        protected CommandHandlerBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            CacheService = ServiceProvider.GetRequiredService<ICacheService>();
            UnitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();
            Logger = ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType());
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Handles the command when invoked by the application pipeline.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>A <see cref="Unit"/> when handling completes successfully.</returns>
        protected abstract Task<Unit> HandleAsync(TCommand command, CancellationToken cancellationToken);

        #endregion Utils

        #region Properties

        /// <summary>
        /// Gets the cache service used for caching operations.
        /// </summary>
        protected ICacheService CacheService { get; }

        /// <summary>
        /// Gets the category logger for the concrete handler type.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the root service provider supplied at construction time.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the unit of work used for persistence operations.
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        #endregion Properties

        #region IRequestHandler Members

        /// <inheritdoc />
        Task<Unit> IRequestHandler<TCommand, Unit>.HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            return HandleAsync(command, cancellationToken);
        }

        #endregion IRequestHandler Members
    }

    /// <summary>
    /// Base class for command handlers that return a <typeparamref name="TResult"/>; resolves <see cref="IUnitOfWork"/> and logging from <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command type handled by this handler.</typeparam>
    /// <typeparam name="TResult">The result type produced by handling the command.</typeparam>
    public abstract class CommandHandlerBase<TCommand, TResult> :
        ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase{TCommand, TResult}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to resolve infrastructure dependencies.</param>
        protected CommandHandlerBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            CacheService = ServiceProvider.GetRequiredService<ICacheService>();
            UnitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();
            Logger = ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType());
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Handles the command when invoked by the application pipeline.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The handler result.</returns>
        protected abstract Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);

        #endregion Utils

        #region Properties

        /// <summary>
        /// Gets the cache service used for caching operations.
        /// </summary>
        protected ICacheService CacheService { get; }

        /// <summary>
        /// Gets the category logger for the concrete handler type.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the root service provider supplied at construction time.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the unit of work used for persistence operations.
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        #endregion Properties

        #region IRequestHandler Members

        /// <inheritdoc />
        Task<TResult> IRequestHandler<TCommand, TResult>.HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            return HandleAsync(command, cancellationToken);
        }

        #endregion IRequestHandler Members
    }
}
