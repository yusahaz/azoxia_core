namespace Azoxia.Core.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq.Expressions;

    internal partial class EntityFilterContext<TEntity>
    {
        #region IEntityFilterContext Members

        /// <inheritdoc />
        Task<bool> IEntityFilterContext<TEntity>.AnyAsync(CancellationToken cancellationToken)
            => _entities.AnyAsync(cancellationToken);

        /// <inheritdoc />
        Task<long> IEntityFilterContext<TEntity>.CountAsync(CancellationToken cancellationToken)
            => _entities.LongCountAsync(cancellationToken);

        /// <inheritdoc />
        Task<TEntity?> IEntityFilterContext<TEntity>.FirstOrDefaultAsync(CancellationToken cancellationToken)
            => _entities.FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        async Task<TResult?> IEntityFilterContext<TEntity>.FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
            where TResult : class
            => await _entities.Select(selector).FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        Task<TEntity?> IEntityFilterContext<TEntity>.SingleOrDefaultAsync(CancellationToken cancellationToken)
            => _entities.SingleOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        async Task<TResult?> IEntityFilterContext<TEntity>.SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
            where TResult : class
            => await _entities.Select(selector).SingleOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        async Task<IEnumerable<TEntity>> IEntityFilterContext<TEntity>.ToListAsync(CancellationToken cancellationToken)
        {
            List<TEntity> result = await _entities.ToListAsync(cancellationToken);

            return result;
        }

        /// <inheritdoc />
        async Task<IEnumerable<TResult>> IEntityFilterContext<TEntity>.ToListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
            => await _entities.Select(selector).ToListAsync(cancellationToken);

        #endregion IEntityFilterContext Members
    }
}
