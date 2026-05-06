namespace Azoxia.Core.Domain
{
    using System;

    /// <summary>
    /// Base type for entities that record creation and last modification metadata.
    /// </summary>
    public abstract class AuditableEntityBase :
        EntityBase
    {
        #region Properties

        /// <summary>
        /// Gets the UTC instant when the entity was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Gets the principal that created the entity, if recorded.
        /// </summary>
        public string? CreatedBy { get; private set; }

        /// <summary>
        /// Gets the UTC instant of the last update, if any.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// Gets the principal that last updated the entity, if recorded.
        /// </summary>
        public string? UpdatedBy { get; private set; }

        #endregion Properties
    }
}
