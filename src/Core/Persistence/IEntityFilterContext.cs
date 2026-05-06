namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Fluent query surface for filtering, shaping, and materializing <typeparamref name="TEntity"/> sequences (synchronous).
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public partial interface IEntityFilterContext<TEntity>
        where TEntity : class, IEntity
    {
        #region Methods

        /// <summary>
        /// Determines whether the filtered sequence contains any elements.
        /// </summary>
        /// <returns><c>true</c> if any element exists; otherwise <c>false</c>.</returns>
        bool Any();

        /// <summary>
        /// Continues the query without change tracking for subsequent operations.
        /// </summary>
        /// <returns>A non-tracking filter context.</returns>
        IEntityFilterContext<TEntity> AsNoTracking();

        /// <summary>
        /// Configures EF Core to load related collections using split queries when multiple Includes are composed,
        /// avoiding Cartesian explosion from a single SQL statement with multiple collection joins.
        /// </summary>
        /// <returns>A filter context with split-query semantics applied.</returns>
        IEntityFilterContext<TEntity> AsSplitQuery();

        /// <summary>
        /// Returns the number of elements in the filtered sequence.
        /// </summary>
        /// <returns>The element count.</returns>
        long Count();

        /// <summary>
        /// Narrows the sequence with <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A filter evaluated against entities.</param>
        /// <returns>A new filter context with the predicate applied.</returns>
        IEntityFilterContext<TEntity> Filter(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Narrows the sequence using a dynamic property name and value.
        /// </summary>
        /// <param name="propertyName">The property name to compare.</param>
        /// <param name="value">The value to compare against.</param>
        /// <returns>A new filter context with the constraint applied.</returns>
        IEntityFilterContext<TEntity> Filter(string propertyName, object value);

        /// <summary>
        /// Returns the first element, or <c>null</c> if the sequence is empty.
        /// </summary>
        /// <returns>The first element, or <c>null</c>.</returns>
        TEntity? FirstOrDefault();

        /// <summary>
        /// Projects and returns the first element using <paramref name="selector"/>, or <c>null</c> if empty.
        /// </summary>
        /// <typeparam name="TResult">The projected type.</typeparam>
        /// <param name="selector">A projection evaluated per entity.</param>
        /// <returns>The first projection, or <c>null</c>.</returns>
        TResult? FirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector)
            where TResult : class;

        /// <summary>
        /// Eager-loads <paramref name="navigationProperty"/> for the query.
        /// </summary>
        /// <param name="navigationProperty">The navigation member to include.</param>
        /// <returns>A filter context with the include applied.</returns>
        IEntityFilterContext<TEntity> Include(Expression<Func<TEntity, object>> navigationProperty);

        /// <summary>
        /// Applies ascending order using a provider-specific <paramref name="order"/> expression (for example property names).
        /// </summary>
        /// <param name="order">The order specification.</param>
        /// <returns>A filter context with ordering applied.</returns>
        IEntityFilterContext<TEntity> OrderBy(string order);

        /// <summary>
        /// Applies ascending order using <paramref name="orderBy"/>.
        /// </summary>
        /// <param name="orderBy">The key selector.</param>
        /// <returns>A filter context with ordering applied.</returns>
        IEntityFilterContext<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy);

        /// <summary>
        /// Applies descending order using a provider-specific <paramref name="order"/> expression.
        /// </summary>
        /// <param name="order">The order specification.</param>
        /// <returns>A filter context with ordering applied.</returns>
        IEntityFilterContext<TEntity> OrderByDescending(string order);

        /// <summary>
        /// Applies descending order using <paramref name="orderBy"/>.
        /// </summary>
        /// <param name="orderBy">The key selector.</param>
        /// <returns>A filter context with ordering applied.</returns>
        IEntityFilterContext<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy);

        /// <summary>
        /// Returns the only element, or <c>null</c> if zero or more than one.
        /// </summary>
        /// <returns>The single element, or <c>null</c>.</returns>
        TEntity? SingleOrDefault();

        /// <summary>
        /// Projects and returns the only element using <paramref name="selector"/>, or <c>null</c> if zero or more than one.
        /// </summary>
        /// <typeparam name="TResult">The projected type.</typeparam>
        /// <param name="selector">A projection evaluated per entity.</param>
        /// <returns>The single projection, or <c>null</c>.</returns>
        TResult? SingleOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector)
            where TResult : class;

        /// <summary>
        /// Skips <paramref name="skip"/> elements from the ordered sequence.
        /// </summary>
        /// <param name="skip">The number of elements to skip.</param>
        /// <returns>A filter context with paging applied.</returns>
        IEntityFilterContext<TEntity> Skip(int skip);

        /// <summary>
        /// Takes at most <paramref name="take"/> elements from the sequence.
        /// </summary>
        /// <param name="take">The maximum number of elements.</param>
        /// <returns>A filter context with paging applied.</returns>
        IEntityFilterContext<TEntity> Take(int take);

        /// <summary>
        /// Applies a secondary ascending sort using a provider-specific <paramref name="thenBy"/> expression.
        /// </summary>
        /// <param name="thenBy">The secondary order specification.</param>
        /// <returns>A filter context with compound ordering applied.</returns>
        IEntityFilterContext<TEntity> ThenBy(string thenBy);

        /// <summary>
        /// Applies a secondary ascending sort using <paramref name="orderBy"/>.
        /// </summary>
        /// <param name="orderBy">The secondary key selector.</param>
        /// <returns>A filter context with compound ordering applied.</returns>
        IEntityFilterContext<TEntity> ThenBy(Expression<Func<TEntity, object>> orderBy);

        /// <summary>
        /// Applies a secondary descending sort using a provider-specific <paramref name="thenBy"/> expression.
        /// </summary>
        /// <param name="thenBy">The secondary order specification.</param>
        /// <returns>A filter context with compound ordering applied.</returns>
        IEntityFilterContext<TEntity> ThenByDescending(string thenBy);

        /// <summary>
        /// Applies a secondary descending sort using <paramref name="orderBy"/>.
        /// </summary>
        /// <param name="orderBy">The secondary key selector.</param>
        /// <returns>A filter context with compound ordering applied.</returns>
        IEntityFilterContext<TEntity> ThenByDescending(Expression<Func<TEntity, object>> orderBy);

        /// <summary>
        /// Materializes the filtered sequence as a list.
        /// </summary>
        /// <returns>The list of entities.</returns>
        IEnumerable<TEntity> ToList();

        /// <summary>
        /// Materializes the filtered sequence as a list and exposes the provider-generated query text via <paramref name="query"/>.
        /// </summary>
        /// <param name="query">When the method returns, contains the query text if supported by the provider.</param>
        /// <returns>The list of entities.</returns>
        IEnumerable<TEntity> ToList(out string query);

        /// <summary>
        /// Materializes the sequence as a list of projections using <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TResult">The projected element type.</typeparam>
        /// <param name="selector">A projection evaluated per entity.</param>
        /// <returns>The projected list.</returns>
        IEnumerable<TResult> ToList<TResult>(Expression<Func<TEntity, TResult>> selector);

        #endregion Methods
    }
}
