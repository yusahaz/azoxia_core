namespace Azoxia.Core.Domain
{
    /// <summary>
    /// Implemented by entities that expose a concurrency token (row version).
    /// </summary>
    public interface ISupportedRowVersion
    {
        #region Properties

        /// <summary>
        /// Gets the opaque row-version or concurrency token from persistence.
        /// </summary>
        byte[] RowVersion { get; }

        #endregion Properties
    }
}
