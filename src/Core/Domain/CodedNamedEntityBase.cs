namespace Azoxia.Core.Domain
{
    /// <summary>
    /// Base type for entities identified by a human-readable name and optional description.
    /// </summary>
    public abstract class CodedNamedEntityBase :
        DeletableEntityBase
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedNamedEntityBase"/> class.
        /// </summary>
        protected CodedNamedEntityBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedNamedEntityBase"/> class with name and optional description.
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <param name="description">The optional description.</param>
        protected internal CodedNamedEntityBase(
            string name,
            string? description = null)
        {
            Name = name;
            Description = description;
        }

        #endregion Ctors

        #region Methods

        /// <summary>
        /// Updates the name and optional description.
        /// </summary>
        /// <param name="name">The new name.</param>
        /// <param name="description">The new description, if any.</param>
        protected internal virtual void UpdateName(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the optional longer description.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string Name { get; private set; }

        #endregion Properties
    }
}
