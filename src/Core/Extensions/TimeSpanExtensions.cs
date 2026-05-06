namespace Azoxia.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        #region Methods

        /// <summary>
        /// Returns the larger of two <see cref="TimeSpan"/> values.
        /// </summary>
        /// <param name="value">The first span.</param>
        /// <param name="other">The second span.</param>
        /// <returns>The maximum.</returns>
        public static TimeSpan Max(this TimeSpan value, TimeSpan other)
            => value >= other ? value : other;

        /// <summary>
        /// Returns the smaller of two <see cref="TimeSpan"/> values.
        /// </summary>
        /// <param name="value">The first span.</param>
        /// <param name="other">The second span.</param>
        /// <returns>The minimum.</returns>
        public static TimeSpan Min(this TimeSpan value, TimeSpan other)
            => value <= other ? value : other;

        #endregion Methods
    }
}
