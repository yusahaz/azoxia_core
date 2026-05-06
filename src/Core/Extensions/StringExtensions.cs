namespace Azoxia.Core.Extensions
{
    using System.Linq;
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified string is null or an empty string.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns><c>true</c> if the string is null or an empty string; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this string? value)
            => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the specified string is null, empty, or contains only white-space characters.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns><c>true</c> if the string is null, empty, or white-space; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrWhiteSpace(this string? value)
            => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Returns <c>null</c> when the value is null or empty; otherwise returns the value.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns><c>null</c> or the original string.</returns>
        public static string? NullIfEmpty(this string? value)
            => value.IsNullOrEmpty() ? null : value;

        /// <summary>
        /// Returns <c>null</c> when the value is null, empty, or white-space; otherwise returns the value.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns><c>null</c> or the original string.</returns>
        public static string? NullIfWhiteSpace(this string? value)
            => value.IsNullOrWhiteSpace() ? null : value;

        /// <summary>
        /// Returns <see cref="string.Empty"/> when the value is <c>null</c>; otherwise returns the value.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns><see cref="string.Empty"/> or <paramref name="value"/>.</returns>
        public static string OrEmpty(this string? value)
            => value ?? string.Empty;

        /// <summary>
        /// Returns <paramref name="fallback"/> when the value is null or empty; otherwise returns the value.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="fallback">The fallback when the value is null or empty.</param>
        /// <returns>The original string or <paramref name="fallback"/>.</returns>
        public static string OrIfEmpty(this string? value, string fallback)
            => value.IsNullOrEmpty() ? fallback : value!;

        /// <summary>
        /// Returns <paramref name="fallback"/> when the value is null, empty, or white-space; otherwise returns the value.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="fallback">The fallback when the value has no content.</param>
        /// <returns>The original string or <paramref name="fallback"/>.</returns>
        public static string OrIfWhiteSpace(this string? value, string fallback)
            => value.IsNullOrWhiteSpace() ? fallback : value!;

        /// <summary>
        /// Splits the string by comma, trims each segment, and omits empty entries.
        /// </summary>
        /// <param name="value">The delimited string (for example <c>abc,asd,asda</c>).</param>
        /// <returns>An empty sequence when <paramref name="value"/> is null or white-space; otherwise the segments.</returns>
        /// <remarks>
        /// For other delimiters use <see cref="Split(string?, char)"/> or <see cref="Split(string?, string)"/> (for example <c>StringExtensions.Split(text, '|')</c>).
        /// A call such as <c>text.Split('|')</c> binds to <see cref="string.Split(char, StringSplitOptions)"/> and returns <c>string[]</c>, not <see cref="IEnumerable{T}"/>.
        /// </remarks>
        public static IEnumerable<string> Split(this string? value)
            => Split(value, ',');

        /// <summary>
        /// Splits the string by <paramref name="separator"/>, trims each segment, and omits empty entries.
        /// </summary>
        /// <param name="value">The delimited string.</param>
        /// <param name="separator">The delimiter character.</param>
        /// <returns>An empty sequence when <paramref name="value"/> is null or white-space; otherwise the segments.</returns>
        public static IEnumerable<string> Split(string? value, char separator)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<string>();
            }

            const StringSplitOptions Options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
            return value!.Split(separator, Options);
        }

        /// <summary>
        /// Splits the string by <paramref name="separator"/> (for example <c>", "</c>), trims each segment, and omits empty entries.
        /// </summary>
        /// <param name="value">The delimited string.</param>
        /// <param name="separator">The delimiter string.</param>
        /// <returns>An empty sequence when <paramref name="value"/> is null or white-space; otherwise the segments.</returns>
        /// <exception cref="AzoxiaException">The separator is null or empty.</exception>
        public static IEnumerable<string> Split(string? value, string separator)
        {
            if (separator.IsNullOrEmpty())
            {
                throw new AzoxiaException(AzoxiaErrorCodes.SplitSeparatorInvalid);
            }

            if (value.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<string>();
            }

            const StringSplitOptions Options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
            return value!.Split(separator, Options);
        }

        /// <summary>
        /// Truncates the string to a maximum length. Shorter or equal-length strings are returned unchanged.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="maxLength">Maximum length of the returned string (excluding <paramref name="truncationSuffix"/> when applied).</param>
        /// <param name="truncationSuffix">Suffix appended when truncation occurs.</param>
        /// <returns>The truncated string, or <c>null</c> when <paramref name="value"/> is <c>null</c>.</returns>
        /// <exception cref="AzoxiaException"><paramref name="maxLength"/> is negative.</exception>
        public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "")
        {
            maxLength.ThrowIfOutOfRange(0, int.MaxValue);

            if (value is null)
            {
                return null;
            }

            if (value.Length <= maxLength)
            {
                return value;
            }

            if (truncationSuffix.IsNullOrEmpty())
            {
                return value[..maxLength];
            }

            int take = maxLength - truncationSuffix.Length;
            if (take <= 0)
            {
                return truncationSuffix[..maxLength];
            }

            return string.Concat(value.AsSpan(0, take), truncationSuffix);
        }

        #endregion Methods
    }
}
