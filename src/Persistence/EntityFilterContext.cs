namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Fluent query wrapper over an <see cref="IQueryable{TEntity}"/> implementing <see cref="IEntityFilterContext{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    internal partial class EntityFilterContext<TEntity> :
        IEntityFilterContext<TEntity>
        where TEntity : class, IEntity
    {
        #region Fields

        private IQueryable<TEntity> _entities;

        private readonly ILogger<EntityFilterContext<TEntity>>? _logger;

        private IOrderedQueryable<TEntity>? _orderedEntities;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilterContext{TEntity}"/> class.
        /// </summary>
        /// <param name="entities">The root query.</param>
        /// <param name="logger">Optional logger for query diagnostics.</param>
        public EntityFilterContext(IQueryable<TEntity> entities, ILogger<EntityFilterContext<TEntity>>? logger = null)
        {
            _entities = entities;
            _logger = logger;
        }

        #endregion Ctors

        #region Utils

        private static Expression<Func<TEntity, bool>> BuildFilterExpression(string propertyPath, object value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression body = parameter;

            foreach (string member in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }

            ConstantExpression constant = Expression.Constant(value, body.Type);
            BinaryExpression equal = Expression.Equal(body, constant);

            return Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
        }

        private static Expression<Func<TEntity, object>> BuildOrderExpression(string propertyPath)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression body = parameter;

            foreach (string member in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }

            body = Expression.Convert(body, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(body, parameter);
        }

        #endregion Utils

        #region IEntityFilterContext Members

        /// <inheritdoc/>
        bool IEntityFilterContext<TEntity>.Any()
            => _entities.Any();

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.AsNoTracking()
        {
            _entities = _entities.AsNoTracking();
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.AsSplitQuery()
        {
            _entities = _entities.AsSplitQuery();
            return this;
        }

        /// <inheritdoc/>
        long IEntityFilterContext<TEntity>.Count()
            => _entities.Count();

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.Filter(Expression<Func<TEntity, bool>> predicate)
        {
            _entities = _entities.Where(predicate);

            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.Filter(string propertyName, object value)
        {
            _entities = _entities.Where(BuildFilterExpression(propertyName, value));
            return this;
        }

        /// <inheritdoc/>
        TEntity? IEntityFilterContext<TEntity>.FirstOrDefault()
            => _entities.FirstOrDefault();

        /// <inheritdoc/>
        TResult? IEntityFilterContext<TEntity>.FirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector)
            where TResult : class
            => _entities.Select(selector).FirstOrDefault();

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.Include(Expression<Func<TEntity, object>> navigationProperty)
        {
            _entities = _entities.Include(navigationProperty);
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            _orderedEntities = _entities.OrderBy(orderBy);
            _entities = _orderedEntities;
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.OrderBy(string propertyName)
        {
            _orderedEntities = _entities.OrderBy(BuildOrderExpression(propertyName));
            _entities = _orderedEntities;
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            _orderedEntities = _entities.OrderByDescending(orderBy);
            _entities = _orderedEntities;
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.OrderByDescending(string propertyName)
        {
            _orderedEntities = _entities.OrderByDescending(BuildOrderExpression(propertyName));
            _entities = _orderedEntities;
            return this;
        }

        /// <inheritdoc/>
        TEntity? IEntityFilterContext<TEntity>.SingleOrDefault()
            => _entities.SingleOrDefault();

        /// <inheritdoc/>
        TResult? IEntityFilterContext<TEntity>.SingleOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector)
            where TResult : class
            => _entities.Select(selector).SingleOrDefault();

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.Skip(int skip)
        {
            _entities = _entities.Skip(skip);
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.Take(int take)
        {
            _entities = _entities.Take(take);
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.ThenBy(Expression<Func<TEntity, object>> thenBy)
        {
            _orderedEntities = _orderedEntities?.ThenBy(thenBy) ?? _entities.OrderBy(thenBy);
            _entities = _orderedEntities;

            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.ThenBy(string propertyName)
        {
            _orderedEntities = _orderedEntities?.ThenBy(BuildOrderExpression(propertyName))
                ?? _entities.OrderBy(BuildOrderExpression(propertyName));
            _entities = _orderedEntities;
            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.ThenByDescending(Expression<Func<TEntity, object>> thenBy)
        {
            _orderedEntities = _orderedEntities?.ThenByDescending(thenBy) ?? _entities.OrderByDescending(thenBy);
            _entities = _orderedEntities;

            return this;
        }

        /// <inheritdoc/>
        IEntityFilterContext<TEntity> IEntityFilterContext<TEntity>.ThenByDescending(string propertyName)
        {
            _orderedEntities = _orderedEntities?.ThenByDescending(BuildOrderExpression(propertyName))
                ?? _entities.OrderByDescending(BuildOrderExpression(propertyName));
            _entities = _orderedEntities;
            return this;
        }

        /// <inheritdoc/>
        IEnumerable<TEntity> IEntityFilterContext<TEntity>.ToList()
        {
            string query = _entities.ToQueryString();
            long startedAt = Environment.TickCount64;
            List<TEntity> result = _entities.ToList();
            long elapsedMs = Environment.TickCount64 - startedAt;
            _logger?.LogDebug("Executed query for {EntityName} in {ElapsedMs}ms. SQL: {SqlQuery}", typeof(TEntity).Name, elapsedMs, query);
            return result;
        }

        /// <inheritdoc/>
        IEnumerable<TEntity> IEntityFilterContext<TEntity>.ToList(out string query)
        {
            query = _entities.ToQueryString();
            long startedAt = Environment.TickCount64;
            List<TEntity> result = _entities.ToList();
            long elapsedMs = Environment.TickCount64 - startedAt;
            _logger?.LogDebug("Executed query for {EntityName} in {ElapsedMs}ms. SQL: {SqlQuery}", typeof(TEntity).Name, elapsedMs, query);
            return result;
        }

        /// <inheritdoc/>
        IEnumerable<TResult> IEntityFilterContext<TEntity>.ToList<TResult>(Expression<Func<TEntity, TResult>> selector)
            => _entities.Select(selector).ToList();

        #endregion IEntityFilterContext Members
    }
}
