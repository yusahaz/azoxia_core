namespace Azoxia.Core.Domain
{
    using System;

    /// <summary>
    /// Base type for entities that support soft deletion and deletion audit fields.
    /// </summary>
    public abstract class DeletableEntityBase :
        AuditableEntityBase
    {
        protected internal virtual void Delete()
        {
            if (IsDeleted)
            {
                return;
            }

            IsDeleted = true;
            DeletedAt = TimeProvider.System.GetUtcNow();
        }

        #region Properties

        /// <summary>
        /// Gets the UTC instant when the entity was soft-deleted, if applicable.
        /// </summary>
        public DateTimeOffset? DeletedAt { get; private set; }

        /// <summary>
        /// Gets the principal that performed the soft deletion, if recorded.
        /// </summary>
        public string? DeletedBy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the entity is soft-deleted.
        /// </summary>
        public bool IsDeleted { get; private set; }

        #endregion Properties
    }
}
