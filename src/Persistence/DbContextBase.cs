namespace Azoxia.Core.Persistence
{
    using Azoxia.Core.Persistence.Diagnostics;
    using Microsoft.EntityFrameworkCore;
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
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }
}
