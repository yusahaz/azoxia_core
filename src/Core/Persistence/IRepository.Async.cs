namespace Azoxia.Core.Persistence
{
    using System.Linq.Expressions;

    /// <summary>
    /// Asynchronous read operations for <typeparamref name="TEntity"/> (extends <see cref="IRepository{TEntity}"/>).
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public partial interface IRepository<TEntity>
    {
        #region Methods

        /// <summary>
        /// Determines asynchronously whether the repository contains any entities.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns><c>true</c> if at least one entity exists; otherwise <c>false</c>.</returns>
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines asynchronously whether any entity satisfies <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns><c>true</c> if a match exists; otherwise <c>false</c>.</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the total number of entities.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The entity count.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the number of entities that satisfy <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The matching count.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the first entity, or <c>null</c> if the sequence is empty.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The first entity, or <c>null</c>.</returns>
        Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the first entity matching <paramref name="predicate"/>, or <c>null</c> if none match.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The first matching entity, or <c>null</c>.</returns>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the entity with key <paramref name="id"/>, or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The entity, or <c>null</c>.</returns>
        ValueTask<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the last entity in the default ordering, or <c>null</c> if empty.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The last entity, or <c>null</c>.</returns>
        Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the last entity matching <paramref name="predicate"/>, or <c>null</c> if none match.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The last matching entity, or <c>null</c>.</returns>
        Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the only entity in the sequence, or <c>null</c> if zero or more than one.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The single entity, or <c>null</c>.</returns>
        Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the only entity matching <paramref name="predicate"/>, or <c>null</c> if zero or more than one match.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The single matching entity, or <c>null</c>.</returns>
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
