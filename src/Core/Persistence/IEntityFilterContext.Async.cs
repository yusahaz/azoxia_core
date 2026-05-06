namespace Azoxia.Core.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Asynchronous materialization for <see cref="IEntityFilterContext{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public partial interface IEntityFilterContext<TEntity>
    {
        #region Methods

        /// <summary>
        /// Determines asynchronously whether the filtered sequence contains any elements.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns><c>true</c> if any element exists; otherwise <c>false</c>.</returns>
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the number of elements in the filtered sequence.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The element count.</returns>
        Task<long> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the first element, or <c>null</c> if the sequence is empty.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The first element, or <c>null</c>.</returns>
        Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the first projected value using <paramref name="selector"/>, or <c>null</c> if empty.
        /// </summary>
        /// <typeparam name="TResult">The projected type.</typeparam>
        /// <param name="selector">A projection evaluated per entity.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The first projection, or <c>null</c>.</returns>
        Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
            where TResult : class;

        /// <summary>
        /// Returns asynchronously the only element, or <c>null</c> if zero or more than one.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The single element, or <c>null</c>.</returns>
        Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns asynchronously the only projected value using <paramref name="selector"/>, or <c>null</c> if zero or more than one.
        /// </summary>
        /// <typeparam name="TResult">The projected type.</typeparam>
        /// <param name="selector">A projection evaluated per entity.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The single projection, or <c>null</c>.</returns>
        Task<TResult?> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
            where TResult : class;

        /// <summary>
        /// Materializes asynchronously the filtered sequence as a list.
        /// </summary>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The list of entities.</returns>
        Task<IEnumerable<TEntity>> ToListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Materializes asynchronously the sequence as a list of projections using <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TResult">The projected element type.</typeparam>
        /// <param name="selector">A projection evaluated per entity.</param>
        /// <param name="cancellationToken">A token that can cancel the operation.</param>
        /// <returns>The projected list.</returns>
        Task<IEnumerable<TResult>> ToListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
