namespace Azoxia.Core.Extensions
{
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Provides Unix epoch (seconds or milliseconds since 1970-01-01T00:00:00Z) conversion helpers.
    /// </summary>
    public static class UnixTimeExtensions
    {
        #region Methods

        /// <summary>
        /// Converts a Unix time in milliseconds to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="unixTimeMilliseconds">Milliseconds since the Unix epoch.</param>
        /// <returns>The offset.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTimeOffset ToDateTimeOffsetFromUnixMilliseconds(this int unixTimeMilliseconds)
            => FromUnixMilliseconds(unixTimeMilliseconds);

        /// <summary>
        /// Converts a Unix time in milliseconds to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="unixTimeMilliseconds">Milliseconds since the Unix epoch.</param>
        /// <returns>The offset.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTimeOffset ToDateTimeOffsetFromUnixMilliseconds(this long unixTimeMilliseconds)
            => FromUnixMilliseconds(unixTimeMilliseconds);

        /// <summary>
        /// Converts a Unix time in seconds to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="unixTimeSeconds">Seconds since the Unix epoch.</param>
        /// <returns>The offset.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTimeOffset ToDateTimeOffsetFromUnixSeconds(this int unixTimeSeconds)
            => FromUnixSeconds(unixTimeSeconds);

        /// <summary>
        /// Converts a Unix time in seconds to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="unixTimeSeconds">Seconds since the Unix epoch.</param>
        /// <returns>The offset.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTimeOffset ToDateTimeOffsetFromUnixSeconds(this long unixTimeSeconds)
            => FromUnixSeconds(unixTimeSeconds);

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Unix time in milliseconds using the same rules as the <see cref="DateTimeOffset"/> constructor.
        /// </summary>
        /// <param name="value">The instant.</param>
        /// <returns>Milliseconds since the Unix epoch.</returns>
        /// <exception cref="AzoxiaException">The value cannot be represented in Unix milliseconds.</exception>
        public static long ToUnixTimeMilliseconds(this DateTime value)
            => ToUnixMilliseconds(value);

        /// <summary>
        /// Converts the <see cref="DateTime"/> to Unix time in seconds using the same rules as the <see cref="DateTimeOffset"/> constructor.
        /// </summary>
        /// <param name="value">The instant.</param>
        /// <returns>Seconds since the Unix epoch.</returns>
        /// <exception cref="AzoxiaException">The value cannot be represented in Unix seconds.</exception>
        public static long ToUnixTimeSeconds(this DateTime value)
            => ToUnixSeconds(value);

        /// <summary>
        /// Converts a Unix time in milliseconds to a <see cref="DateTime"/> in UTC (<see cref="DateTimeKind.Utc"/>).
        /// </summary>
        /// <param name="unixTimeMilliseconds">Milliseconds since the Unix epoch.</param>
        /// <returns>The UTC <see cref="DateTime"/>.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTime ToUtcDateTimeFromUnixMilliseconds(this int unixTimeMilliseconds)
            => FromUnixMilliseconds(unixTimeMilliseconds).UtcDateTime;

        /// <summary>
        /// Converts a Unix time in milliseconds to a <see cref="DateTime"/> in UTC (<see cref="DateTimeKind.Utc"/>).
        /// </summary>
        /// <param name="unixTimeMilliseconds">Milliseconds since the Unix epoch.</param>
        /// <returns>The UTC <see cref="DateTime"/>.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTime ToUtcDateTimeFromUnixMilliseconds(this long unixTimeMilliseconds)
            => FromUnixMilliseconds(unixTimeMilliseconds).UtcDateTime;

        /// <summary>
        /// Converts a Unix time in seconds to a <see cref="DateTime"/> in UTC (<see cref="DateTimeKind.Utc"/>).
        /// </summary>
        /// <param name="unixTimeSeconds">Seconds since the Unix epoch.</param>
        /// <returns>The UTC <see cref="DateTime"/>.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTime ToUtcDateTimeFromUnixSeconds(this int unixTimeSeconds)
            => FromUnixSeconds(unixTimeSeconds).UtcDateTime;

        /// <summary>
        /// Converts a Unix time in seconds to a <see cref="DateTime"/> in UTC (<see cref="DateTimeKind.Utc"/>).
        /// </summary>
        /// <param name="unixTimeSeconds">Seconds since the Unix epoch.</param>
        /// <returns>The UTC <see cref="DateTime"/>.</returns>
        /// <exception cref="AzoxiaException">The value is outside the range supported by <see cref="DateTimeOffset"/>.</exception>
        public static DateTime ToUtcDateTimeFromUnixSeconds(this long unixTimeSeconds)
            => FromUnixSeconds(unixTimeSeconds).UtcDateTime;

        #endregion Methods

        #region Utils

        private static DateTimeOffset FromUnixMilliseconds(long unixTimeMilliseconds)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.UnixTimeOutOfRange, ex);
            }
        }

        private static DateTimeOffset FromUnixSeconds(long unixTimeSeconds)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.UnixTimeOutOfRange, ex);
            }
        }

        private static long ToUnixMilliseconds(DateTime value)
        {
            try
            {
                return new DateTimeOffset(value).ToUnixTimeMilliseconds();
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException or ArgumentException)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.UnixTimeOutOfRange, ex);
            }
        }

        private static long ToUnixSeconds(DateTime value)
        {
            try
            {
                return new DateTimeOffset(value).ToUnixTimeSeconds();
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException or ArgumentException)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.UnixTimeOutOfRange, ex);
            }
        }

        #endregion Utils
    }
}
