namespace Azoxia.Core.Exceptions
{
    /// <summary>
    /// Base exception type for the Azoxia Core layer. Messages are supplied only via <see cref="ErrorCode"/>.
    /// </summary>
    public class AzoxiaException : Exception
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance of <see cref="AzoxiaException"/>.
        /// </summary>
        /// <param name="error">Defined error information.</param>
        public AzoxiaException(ErrorCode error)
            : base(error.ErrorMessage)
        {
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AzoxiaException"/> with an inner exception.
        /// </summary>
        /// <param name="error">Defined error information.</param>
        /// <param name="innerException">The inner exception.</param>
        public AzoxiaException(ErrorCode error, Exception innerException)
            : base(error.ErrorMessage, innerException)
        {
            Error = error;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Gets the error code and message for this exception.
        /// </summary>
        public ErrorCode Error { get; }

        #endregion Properties
    }
}
