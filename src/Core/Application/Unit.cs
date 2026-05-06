namespace Azoxia.Core.Application
{
    /// <summary>
    /// Canonical empty result for commands that do not yield a value (similar to <c>void</c> in the request/handler pipeline).
    /// </summary>
    public readonly record struct Unit
    {
        #region Methods

        /// <inheritdoc />
        public override string ToString() => "()";

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the singleton <see cref="Unit"/> value used by handlers that complete without data.
        /// </summary>
        public static readonly Unit Value = new();

        #endregion Properties
    }
}
