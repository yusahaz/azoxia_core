namespace Azoxia.Core.Exceptions
{
    /// <summary>
    /// Carries a defined error code and a fixed message.
    /// </summary>
    /// <param name="Code">Stable error identifier (e.g. AZX_CORE_001).</param>
    /// <param name="ErrorMessage">Human-readable error description.</param>
    public readonly record struct ErrorCode(string Code, string ErrorMessage);
}
