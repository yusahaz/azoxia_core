namespace Azoxia.Core.Domain
{
    /// <summary>
    /// Base type for domain entities with a stable identifier.
    /// </summary>
    public abstract class EntityBase :
        IEntity
    {
        #region Properties

        /// <summary>
        /// Gets the persisted entity identifier.
        /// </summary>
        public int Id { get; private set; }

        #endregion Properties
    }
}
