namespace Azoxia.Core.Persistence
{
    using Microsoft.EntityFrameworkCore;

    using System.Linq.Expressions;

    internal partial class Repository<TEntity>
    {
        #region Methods

        /// <inheritdoc />
        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
            => Entities.AnyAsync(cancellationToken);

        /// <inheritdoc />
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => Entities.AnyAsync(predicate, cancellationToken);

        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken = default)
            => Entities.CountAsync(cancellationToken);

        /// <inheritdoc />
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => Entities.CountAsync(predicate, cancellationToken);

        /// <inheritdoc />
        public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
            => Entities.FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => Entities.FirstOrDefaultAsync(predicate, cancellationToken);

        /// <inheritdoc />
        public ValueTask<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _dbContext.FindAsync<TEntity>(keyValues: [id], cancellationToken: cancellationToken);

        /// <inheritdoc />
        public Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default)
            => Entities.LastOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        public Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => Entities.LastOrDefaultAsync(predicate, cancellationToken);

        /// <inheritdoc />
        public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
            => Entities.SingleOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => Entities.SingleOrDefaultAsync(predicate, cancellationToken);

        #endregion Methods
    }
}
