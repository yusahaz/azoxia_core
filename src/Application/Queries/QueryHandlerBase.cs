namespace Azoxia.Core.Application.Queries
{
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Persistence;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;

    public abstract class QueryHandlerBase<TQuery, TResult> :
        IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        #region Ctors


        protected QueryHandlerBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            CacheService = ServiceProvider.GetRequiredService<ICacheService>();
            UnitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();
            Logger = ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType());
        }

        #endregion Ctors

        #region Utils


        protected abstract Task<TResult> HandleAsync(TQuery Query, CancellationToken cancellationToken);

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
        Task<TResult> IRequestHandler<TQuery, TResult>.HandleAsync(TQuery Query, CancellationToken cancellationToken)
        {
            return HandleAsync(Query, cancellationToken);
        }

        #endregion IRequestHandler Members
    }
}
