namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Exceptions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Scoped unit-of-work facade over a <see cref="DbContext"/> with centralized exception mapping on save.
    /// </summary>
    /// <typeparam name="TDbContext">The EF Core context type.</typeparam>
    public class UnitOfWork<TDbContext>(TDbContext dbContext, ILogger<UnitOfWork<TDbContext>> logger) :
        IUnitOfWork
        where TDbContext : DbContext
    {
        #region Fields

        private readonly ConcurrentDictionary<Type, Lazy<object>> _repositories = new();

        #endregion Fields

        #region IUnitOfWork Members

        /// <inheritdoc />
        void IUnitOfWork.Add<TEntity>(TEntity entity)
            => dbContext.Add(entity);

        /// <inheritdoc />
        void IUnitOfWork.AddRange<TEntity>(IEnumerable<TEntity> entities)
            => dbContext.AddRange(entities);

        /// <inheritdoc />
        void IUnitOfWork.Delete<TEntity>(TEntity entity)
            => dbContext.Remove(entity);

        /// <inheritdoc />
        void IUnitOfWork.DeleteRange<TEntity>(IEnumerable<TEntity> entities)
            => dbContext.RemoveRange(entities);

        /// <inheritdoc />
        IRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
        {
            Lazy<object> repository = _repositories.GetOrAdd(
                typeof(TEntity),
                _ => new Lazy<object>(() => new Repository<TEntity>(dbContext)));

            return (IRepository<TEntity>)repository.Value;
        }

        /// <inheritdoc />
        async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("UnitOfWork transaction completed successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex, "Concurrency conflict detected during SaveChangesAsync.");
                throw new AzoxiaException(AzoxiaErrorCodes.PersistenceConcurrencyConflict, ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogWarning(ex, "Database update failure occurred during SaveChangesAsync.");
                throw new AzoxiaException(AzoxiaErrorCodes.PersistenceSaveFailed, ex);
            }
            catch (TimeoutException ex)
            {
                logger.LogWarning(ex, "Database timeout occurred during SaveChangesAsync.");
                throw new AzoxiaException(AzoxiaErrorCodes.PersistenceTimeout, ex);
            }
        }

        /// <inheritdoc />
        void IUnitOfWork.Update<TEntity>(TEntity entity)
            => dbContext.Update(entity);

        /// <inheritdoc />
        void IUnitOfWork.UpdateRange<TEntity>(IEnumerable<TEntity> entities)
            => dbContext.UpdateRange(entities);

        #endregion IUnitOfWork Members
    }
}
