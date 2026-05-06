namespace Azoxia.Core.Extensions
{
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Helpers to throw <see cref="AzoxiaException"/> from an <see cref="ErrorCode"/>.
    /// </summary>
    public static class ErrorCodeExtensions
    {
        #region Methods

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> for this error code.
        /// </summary>
        /// <param name="error">Defined error.</param>
        /// <exception cref="AzoxiaException">Always thrown.</exception>
        public static void Throw(this ErrorCode error)
            => throw new AzoxiaException(error);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> for this error code with an inner exception.
        /// </summary>
        /// <param name="error">Defined error.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <exception cref="AzoxiaException">Always thrown.</exception>
        public static void Throw(this ErrorCode error, Exception innerException)
            => throw new AzoxiaException(error, innerException);

        #endregion Methods
    }
}
