namespace Azoxia.Core.Extensions
{
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Guard-style precondition checks; throws using only <see cref="ErrorCode"/> (no ad-hoc exception message strings).
    /// </summary>
    public static class GuardExtensions
    {
        #region Methods

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the condition is false.
        /// </summary>
        /// <param name="condition">The condition expected to be true.</param>
        /// <param name="error">Defined error.</param>
        /// <exception cref="AzoxiaException">The condition is false.</exception>
        public static void ThrowIfFalse(this bool condition, ErrorCode error)
        {
            if (!condition)
            {
                throw new AzoxiaException(error);
            }
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.ExpectedConditionTrue"/> if the condition is false.
        /// </summary>
        /// <param name="condition">The condition expected to be true.</param>
        /// <exception cref="AzoxiaException">The condition is false.</exception>
        public static void ThrowIfFalse(this bool condition)
            => condition.ThrowIfFalse(AzoxiaErrorCodes.ExpectedConditionTrue);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the value is null; otherwise invokes <paramref name="action"/> with the non-null value.
        /// </summary>
        /// <typeparam name="T">Reference type.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="action">Action to run with the non-null value.</param>
        /// <param name="error">Defined error when <paramref name="value"/> is null.</param>
        /// <exception cref="AzoxiaException">The value or <paramref name="action"/> is null.</exception>
        public static void ThrowIfNull<T>(this T? value, Action<T> action, ErrorCode error)
            where T : class
        {
            action.ThrowIfNull();
            T nonNull = value.ThrowIfNull(error);
            action(nonNull);
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the value is null.
        /// </summary>
        /// <typeparam name="T">Reference type.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="error">Defined error.</param>
        /// <returns>The non-null value.</returns>
        /// <exception cref="AzoxiaException">The value is null.</exception>
        public static T ThrowIfNull<T>(this T? value, ErrorCode error)
            where T : class
        {
            (value is not null).ThrowIfFalse(error);
            return value!;
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.ArgumentNull"/> if the value is null.
        /// </summary>
        /// <typeparam name="T">Reference type.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <returns>The non-null value.</returns>
        /// <exception cref="AzoxiaException">The value is null.</exception>
        public static T ThrowIfNull<T>(this T? value)
            where T : class
            => value.ThrowIfNull(AzoxiaErrorCodes.ArgumentNull);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the nullable value has no value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="error">Defined error.</param>
        /// <returns>The contained value.</returns>
        /// <exception cref="AzoxiaException">The nullable has no value.</exception>
        public static T ThrowIfNull<T>(this T? value, ErrorCode error)
            where T : struct
        {
            value.HasValue.ThrowIfFalse(error);
            return value!.Value;
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.NullableValueMissing"/> if the nullable value has no value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <returns>The contained value.</returns>
        /// <exception cref="AzoxiaException">The nullable has no value.</exception>
        public static T ThrowIfNull<T>(this T? value)
            where T : struct
            => value.ThrowIfNull(AzoxiaErrorCodes.NullableValueMissing);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the string is null or empty.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <param name="error">Defined error.</param>
        /// <returns>The non-null string.</returns>
        /// <exception cref="AzoxiaException">The string is null or empty.</exception>
        public static string ThrowIfNullOrEmpty(this string? value, ErrorCode error)
        {
            (!value.IsNullOrEmpty()).ThrowIfFalse(error);
            return value!;
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.StringNullOrEmpty"/> if the string is null or empty.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>The non-null string.</returns>
        /// <exception cref="AzoxiaException">The string is null or empty.</exception>
        public static string ThrowIfNullOrEmpty(this string? value)
            => value.ThrowIfNullOrEmpty(AzoxiaErrorCodes.StringNullOrEmpty);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the string is null, empty, or white-space only.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <param name="error">Defined error.</param>
        /// <returns>The non-null string.</returns>
        /// <exception cref="AzoxiaException">The string is not usable.</exception>
        public static string ThrowIfNullOrWhiteSpace(this string? value, ErrorCode error)
        {
            (!value.IsNullOrWhiteSpace()).ThrowIfFalse(error);
            return value!;
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.StringNullOrWhiteSpace"/> if the string is null, empty, or white-space only.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>The non-null string.</returns>
        /// <exception cref="AzoxiaException">The string is not usable.</exception>
        public static string ThrowIfNullOrWhiteSpace(this string? value)
            => value.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.ArgumentOutOfRange"/> if the value is outside the inclusive range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Inclusive upper bound.</param>
        /// <returns><paramref name="value"/>.</returns>
        /// <exception cref="AzoxiaException">The value is out of range.</exception>
        public static int ThrowIfOutOfRange(this int value, int min, int max)
            => value.ThrowIfOutOfRange(min, max, AzoxiaErrorCodes.ArgumentOutOfRange);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the value is outside the inclusive range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Inclusive upper bound.</param>
        /// <param name="error">Defined error.</param>
        /// <returns><paramref name="value"/>.</returns>
        /// <exception cref="AzoxiaException">The value is out of range.</exception>
        public static int ThrowIfOutOfRange(this int value, int min, int max, ErrorCode error)
        {
            (value >= min && value <= max).ThrowIfFalse(error);
            return value;
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.ArgumentOutOfRange"/> if the value is outside the inclusive range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Inclusive upper bound.</param>
        /// <returns><paramref name="value"/>.</returns>
        /// <exception cref="AzoxiaException">The value is out of range.</exception>
        public static double ThrowIfOutOfRange(this double value, double min, double max)
            => value.ThrowIfOutOfRange(min, max, AzoxiaErrorCodes.ArgumentOutOfRange);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the value is outside the inclusive range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Inclusive upper bound.</param>
        /// <param name="error">Defined error.</param>
        /// <returns><paramref name="value"/>.</returns>
        /// <exception cref="AzoxiaException">The value is out of range.</exception>
        public static double ThrowIfOutOfRange(this double value, double min, double max, ErrorCode error)
        {
            (value >= min && value <= max).ThrowIfFalse(error);
            return value;
        }

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> if the condition is true.
        /// </summary>
        /// <param name="condition">The condition expected to be false.</param>
        /// <param name="error">Defined error.</param>
        /// <exception cref="AzoxiaException">The condition is true.</exception>
        public static void ThrowIfTrue(this bool condition, ErrorCode error)
            => (!condition).ThrowIfFalse(error);

        /// <summary>
        /// Throws <see cref="AzoxiaException"/> with <see cref="AzoxiaErrorCodes.ExpectedConditionFalse"/> if the condition is true.
        /// </summary>
        /// <param name="condition">The condition expected to be false.</param>
        /// <exception cref="AzoxiaException">The condition is true.</exception>
        public static void ThrowIfTrue(this bool condition)
            => condition.ThrowIfTrue(AzoxiaErrorCodes.ExpectedConditionFalse);

        #endregion Methods
    }
}
