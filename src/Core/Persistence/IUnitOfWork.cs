namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Coordinates repositories and persists aggregate changes as a unit.
    /// </summary>
    public interface IUnitOfWork
    {
        #region Methods

        /// <summary>
        /// Marks <paramref name="entity"/> for insertion when changes are saved.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to add.</param>
        void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        /// <summary>
        /// Marks <paramref name="entities"/> for insertion when changes are saved.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entities">The entities to add.</param>
        void AddRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity;

        /// <summary>
        /// Marks <paramref name="entity"/> for deletion when changes are saved.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        /// <summary>
        /// Marks <paramref name="entities"/> for deletion when changes are saved.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entities">The entities to delete.</param>
        void DeleteRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity;

        /// <summary>
        /// Resolves the repository for <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>The repository instance.</returns>
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IEntity;

        /// <summary>
        /// Persists all pending unit-of-work changes.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks <paramref name="entity"/> as modified when changes are saved.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to update.</param>
        void Update<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        /// <summary>
        /// Marks <paramref name="entities"/> as modified when changes are saved.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entities">The entities to update.</param>
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity;

        #endregion Methods
    }
}
