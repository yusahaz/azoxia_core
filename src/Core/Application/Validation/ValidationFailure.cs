namespace Azoxia.Core.Application.Validation
{
    /// <summary>
    /// Single validation error detail.
    /// </summary>
    /// <param name="Field">Field/property name.</param>
    /// <param name="Code">Stable validation code.</param>
    /// <param name="Message">Human-readable description.</param>
    public readonly record struct ValidationFailure(string Field, string Code, string Message);
}
