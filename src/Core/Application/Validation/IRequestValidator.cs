namespace Azoxia.Core.Application.Validation
{
    /// <summary>
    /// Validates an application request before handler execution.
    /// </summary>
    /// <typeparam name="TRequest">Request type.</typeparam>
    public interface IRequestValidator<in TRequest>
        where TRequest : class
    {
        #region Methods

        /// <summary>
        /// Runs validation and returns failures, if any.
        /// </summary>
        ValidationResult Validate(TRequest request);

        #endregion Methods
    }
}
