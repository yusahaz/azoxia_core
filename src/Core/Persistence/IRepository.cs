namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Domain;

    using System.Linq.Expressions;

    /// <summary>
    /// Read and filter operations for <typeparamref name="TEntity"/> instances (synchronous surface).
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public partial interface IRepository<TEntity>
        where TEntity : class, IEntity
    {
        #region Methods

        /// <summary>
        /// Determines whether the repository contains any entities.
        /// </summary>
        /// <returns><c>true</c> if at least one entity exists; otherwise <c>false</c>.</returns>
        bool Any();

        /// <summary>
        /// Determines whether any entity satisfies <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns><c>true</c> if a match exists; otherwise <c>false</c>.</returns>
        bool Any(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns a repository view that does not track returned entities (read-only queries).
        /// </summary>
        /// <returns>A non-tracking repository handle.</returns>
        IRepository<TEntity> AsNoTracking();

        /// <summary>
        /// Returns the total number of entities.
        /// </summary>
        /// <returns>The entity count.</returns>
        int Count();

        /// <summary>
        /// Returns the number of entities that satisfy <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns>The matching count.</returns>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Starts a fluent filter without a predicate (provider-specific; may represent “all” or a root query).
        /// </summary>
        /// <returns>A filter context for further composition.</returns>
        IEntityFilterContext<TEntity> Filter();

        /// <summary>
        /// Starts a fluent filter with <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns>A filter context for further composition.</returns>
        IEntityFilterContext<TEntity> Filter(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Starts a fluent filter using a dynamic property name and value (provider-specific).
        /// </summary>
        /// <param name="propertyName">The property name to compare.</param>
        /// <param name="value">The value to compare against.</param>
        /// <returns>A filter context for further composition.</returns>
        IEntityFilterContext<TEntity> Filter(string propertyName, object value);

        /// <summary>
        /// Returns the first entity, or <c>null</c> if the sequence is empty.
        /// </summary>
        /// <returns>The first entity, or <c>null</c>.</returns>
        TEntity? FirstOrDefault();

        /// <summary>
        /// Returns the first entity matching <paramref name="predicate"/>, or <c>null</c> if none match.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns>The first matching entity, or <c>null</c>.</returns>
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns the entity with key <paramref name="id"/>, or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <returns>The entity, or <c>null</c>.</returns>
        TEntity? GetById(int id);

        /// <summary>
        /// Returns the last entity in the default ordering, or <c>null</c> if empty.
        /// </summary>
        /// <returns>The last entity, or <c>null</c>.</returns>
        TEntity? LastOrDefault();

        /// <summary>
        /// Returns the last entity matching <paramref name="predicate"/>, or <c>null</c> if none match.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns>The last matching entity, or <c>null</c>.</returns>
        TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns the only entity in the sequence, or <c>null</c> if zero or more than one.
        /// </summary>
        /// <returns>The single entity, or <c>null</c>.</returns>
        TEntity? SingleOrDefault();

        /// <summary>
        /// Returns the only entity matching <paramref name="predicate"/>, or <c>null</c> if zero or more than one match.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns>The single matching entity, or <c>null</c>.</returns>
        TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        #endregion Methods
    }
}
