namespace Azoxia.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Methods

        /// <summary>
        /// Adds the specified number of weeks to the value.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <param name="weeks">The number of weeks.</param>
        /// <returns>The result.</returns>
        public static DateTime AddWeeks(this DateTime value, int weeks)
            => value.AddDays(7 * weeks);

        /// <summary>
        /// Returns the last tick of the calendar day for <paramref name="value"/> (23:59:59.9999999) with the same <see cref="DateTime.Kind"/>.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <returns>The end of the day.</returns>
        public static DateTime EndOfDay(this DateTime value)
            => value.Date.AddDays(1).AddTicks(-1);

        /// <summary>
        /// Returns the last tick of the last day in the month of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <returns>The end of the month.</returns>
        public static DateTime EndOfMonth(this DateTime value)
            => value.StartOfMonth().AddMonths(1).AddTicks(-1);

        /// <summary>
        /// Determines whether the date falls on Saturday or Sunday.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <returns><c>true</c> if the day is weekend; otherwise, <c>false</c>.</returns>
        public static bool IsWeekend(this DateTime value)
            => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        /// <summary>
        /// Returns midnight at the start of the calendar day for <paramref name="value"/> with the same <see cref="DateTime.Kind"/>.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <returns>The start of the day.</returns>
        public static DateTime StartOfDay(this DateTime value)
            => value.Date;

        /// <summary>
        /// Returns midnight at the first day of the month for <paramref name="value"/> with the same <see cref="DateTime.Kind"/>.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <returns>The start of the month.</returns>
        public static DateTime StartOfMonth(this DateTime value)
            => new DateTime(value.Year, value.Month, 1, 0, 0, 0, 0, value.Kind);

        #endregion Methods
    }
}
