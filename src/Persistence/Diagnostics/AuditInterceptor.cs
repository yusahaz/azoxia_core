namespace Azoxia.Core.Persistence.Diagnostics
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Identity;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// EF Core save interceptor that stamps audit columns on <see cref="AuditableEntityBase"/> (and soft-delete fields on <see cref="DeletableEntityBase"/>).
    /// </summary>
    internal class AuditInterceptor :
        SaveChangesInterceptor
    {
        #region Utils

        private string BuildDelta(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var sb = new StringBuilder();

            foreach (var prop in entry.Properties)
            {
                if (!prop.IsModified)
                {
                    continue;
                }

                string name = prop.Metadata.Name;

                if (name is nameof(AuditableEntityBase.UpdatedBy)
                    or nameof(AuditableEntityBase.UpdatedAt))
                {
                    continue;
                }

                sb.Append($"{name}: '{prop.OriginalValue}' → '{prop.CurrentValue}'; ");
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "(no tracked changes)";
        }

        private void SetAudit(DbContext dbContext)
        {
            if (dbContext is null)
            {
                return;
            }

            IExecutionContext executionContext = dbContext.GetService<IExecutionContext>();
            ILogger<AuditInterceptor>? logger = dbContext.GetService<ILogger<AuditInterceptor>>();

            string username = executionContext?.IsAuthenticated == true
                ? executionContext.GetClaim(ClaimTypes.Name) ?? "System"
                : "System";

            DateTimeOffset now = DateTimeOffset.UtcNow;

            var auditableEntries = dbContext.ChangeTracker
                .Entries<AuditableEntityBase>()
                .Where(x => x.State is EntityState.Added or EntityState.Modified);

            foreach (var entry in auditableEntries)
            {
                if (entry.State is EntityState.Added)
                {
                    entry.Property(x => x.CreatedBy).CurrentValue = username;
                    entry.Property(x => x.CreatedAt).CurrentValue = now;

                    logger?.LogDebug(
                        "Audit [ADD] {EntityType} by {User} at {Timestamp}.",
                        entry.Entity.GetType().Name,
                        username,
                        now);
                }

                if (entry.State is EntityState.Modified)
                {
                    entry.Property(x => x.UpdatedBy).CurrentValue = username;
                    entry.Property(x => x.UpdatedAt).CurrentValue = now;

                    if (entry.Entity is DeletableEntityBase deletable && deletable.IsDeleted)
                    {
                        entry.Property(nameof(DeletableEntityBase.DeletedBy)).CurrentValue = username;

                        logger?.LogDebug(
                            "Audit [DELETE] {EntityType} Id={EntityId} by {User} at {Timestamp}.",
                            entry.Entity.GetType().Name,
                            entry.Property(nameof(EntityBase.Id)).CurrentValue,
                            username,
                            now);
                    }
                    else
                    {
                        string delta = BuildDelta(entry);

                        logger?.LogDebug(
                            "Audit [UPDATE] {EntityType} Id={EntityId} by {User} at {Timestamp}. Changes: {Delta}",
                            entry.Entity.GetType().Name,
                            entry.Property(nameof(EntityBase.Id)).CurrentValue,
                            username,
                            now,
                            delta);
                    }
                }
            }
        }

        #endregion Utils

        #region Methods

        /// <inheritdoc />
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        /// <inheritdoc />
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SetAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        #endregion Methods
    }
}
