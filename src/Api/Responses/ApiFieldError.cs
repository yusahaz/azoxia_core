namespace Azoxia.Core.Api.Responses
{
    /// <summary>
    /// A single field-level validation or binding error carried inside an <see cref="ApiResponse"/>.
    /// </summary>
    /// <param name="Field">Source field or model path.</param>
    /// <param name="Code">Stable machine code for the failure.</param>
    /// <param name="Message">Human-readable explanation.</param>
    public sealed record ApiFieldError(string Field, string Code, string Message);
}
