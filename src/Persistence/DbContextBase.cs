namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Persistence.Diagnostics;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Application <see cref="DbContext"/> base that wires auditing, EF mapping discovery, and repository access patterns.
    /// </summary>
    /// <typeparam name="TDBContext">The concrete derived context type.</typeparam>
    public abstract class DbContextBase<TDBContext>(DbContextOptions<TDBContext> options) :
        DbContext(options)
        where TDBContext : DbContext
    {
        #region Methods

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new AuditInterceptor());
            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Assembly persistenceAssembly = typeof(DbContextBase<>).Assembly;
            Assembly contextAssembly = typeof(TDBContext).Assembly;

            if (!ReferenceEquals(persistenceAssembly, contextAssembly))
            {
                modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(contextAssembly);
            ApplySoftDeleteQueryFilters(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                Type? clrType = entityType.ClrType;
                if (clrType is null || !typeof(DeletableEntityBase).IsAssignableFrom(clrType))
                {
                    continue;
                }

                ParameterExpression parameter = Expression.Parameter(clrType, "e");
                MethodCallExpression isDeletedProperty =
                    Expression.Call(
                        typeof(EF),
                        nameof(EF.Property),
                        [typeof(bool)],
                        parameter,
                        Expression.Constant(nameof(DeletableEntityBase.IsDeleted)));

                BinaryExpression notDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                LambdaExpression filter = Expression.Lambda(notDeleted, parameter);

                entityType.SetQueryFilter(filter);
            }
        }

        #endregion Methods
    }
}
