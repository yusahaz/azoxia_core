namespace Azoxia.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for byte arrays.
    /// </summary>
    public static class ByteExtensions
    {
        #region Methods

        /// <summary>
        /// Converts the bytes to a Base-64 string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="options">Formatting options.</param>
        /// <returns>The Base-64 string.</returns>
        /// <exception cref="AzoxiaException"><paramref name="bytes"/> is <c>null</c>.</exception>
        public static string ToBase64String(this byte[] bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
            bytes.ThrowIfNull();
            return Convert.ToBase64String(bytes, options);
        }

        /// <summary>
        /// Converts the bytes to a hexadecimal string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="lowerCase">When <c>true</c>, uses lowercase letters <c>a</c>-<c>f</c>.</param>
        /// <returns>The hexadecimal string.</returns>
        /// <exception cref="AzoxiaException"><paramref name="bytes"/> is <c>null</c>.</exception>
        public static string ToHexString(this byte[] bytes, bool lowerCase = false)
        {
            bytes.ThrowIfNull();
            string hex = Convert.ToHexString(bytes);
            return lowerCase ? hex.ToLowerInvariant() : hex;
        }

        #endregion Methods
    }
}
