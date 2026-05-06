namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Domain;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Logging;

    using System.Linq.Expressions;

    /// <summary>
    /// Default <see cref="IRepository{TEntity}"/> implementation backed by a <see cref="DbContext"/> set.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    internal partial class Repository<TEntity> :
        IRepository<TEntity>
        where TEntity : class, IEntity
    {
        #region Fields

        private readonly DbContext _dbContext;

        private readonly ILogger<EntityFilterContext<TEntity>>? _entityFilterLogger;

        private IQueryable<TEntity> _entities;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="dbContext">The EF Core context.</param>
        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _entityFilterLogger = _dbContext.GetService<ILoggerFactory>()
                ?.CreateLogger<EntityFilterContext<TEntity>>();
        }

        #endregion Ctors

        #region Methods

        /// <inheritdoc />
        public bool Any()
            => Entities.Any();

        /// <inheritdoc />
        public bool Any(Expression<Func<TEntity, bool>> predicate)
            => Entities.Any(predicate);

        /// <inheritdoc />
        public IRepository<TEntity> AsNoTracking()
        {
            _entities = _entities.AsNoTracking();
            return this;
        }

        /// <inheritdoc />
        public int Count()
            => Entities.Count();

        /// <inheritdoc />
        public int Count(Expression<Func<TEntity, bool>> predicate)
            => Entities.Count(predicate);

        /// <inheritdoc />
        public TEntity? FirstOrDefault()
            => Entities.FirstOrDefault();

        /// <inheritdoc />
        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
            => Entities.FirstOrDefault(predicate);

        /// <inheritdoc />
        public TEntity? GetById(int id)
            => _dbContext.Find<TEntity>(id);

        /// <inheritdoc />
        public TEntity? LastOrDefault()
            => Entities.LastOrDefault();

        /// <inheritdoc />
        public TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate)
            => Entities.LastOrDefault(predicate);

        /// <inheritdoc />
        public TEntity? SingleOrDefault()
            => Entities.SingleOrDefault();

        /// <inheritdoc />
        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
            => Entities.SingleOrDefault(predicate);

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the tracked or configured query for this entity type.
        /// </summary>
        protected IQueryable<TEntity> Entities => _entities ??= _dbContext.Set<TEntity>();

        #endregion Properties

        #region IRepository Members

        /// <inheritdoc />
        IEntityFilterContext<TEntity> IRepository<TEntity>.Filter()
            => new EntityFilterContext<TEntity>(Entities, _entityFilterLogger);

        /// <inheritdoc />
        IEntityFilterContext<TEntity> IRepository<TEntity>.Filter(Expression<Func<TEntity, bool>> predicate)
            => ((IEntityFilterContext<TEntity>)new EntityFilterContext<TEntity>(Entities, _entityFilterLogger)).Filter(predicate);

        /// <inheritdoc />
        IEntityFilterContext<TEntity> IRepository<TEntity>.Filter(string propertyName, object value)
            => ((IEntityFilterContext<TEntity>)new EntityFilterContext<TEntity>(Entities, _entityFilterLogger)).Filter(propertyName, value);

        #endregion IRepository Members
    }
}
