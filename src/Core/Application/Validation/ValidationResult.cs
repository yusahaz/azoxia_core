namespace Azoxia.Core.Application.Validation
{
    /// <summary>
    /// Result object returned by request validators.
    /// </summary>
    public sealed class ValidationResult
    {
        #region Ctors

        public ValidationResult(IReadOnlyList<ValidationFailure>? errors = null)
        {
            Errors = errors ?? [];
        }

        #endregion Ctors

        #region Properties

        public IReadOnlyList<ValidationFailure> Errors { get; }

        public bool IsValid => Errors.Count == 0;

        #endregion Properties
    }
}
