namespace Azoxia.Core.Application.Validation
{
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Exception thrown when request validation fails before handler execution.
    /// </summary>
    public class RequestValidationException :
        AzoxiaException
    {
        #region Ctors

        public RequestValidationException(IReadOnlyList<ValidationFailure> failures)
            : base(AzoxiaErrorCodes.RequestValidationFailed)
        {
            Failures = failures;
        }

        #endregion Ctors

        #region Properties

        public IReadOnlyList<ValidationFailure> Failures { get; }

        #endregion Properties
    }
}
