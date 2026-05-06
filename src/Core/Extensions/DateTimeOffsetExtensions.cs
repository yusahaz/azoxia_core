namespace Azoxia.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        #region Methods

        /// <summary>
        /// Adds the specified number of weeks to the value.
        /// </summary>
        /// <param name="value">The date and time with offset.</param>
        /// <param name="weeks">The number of weeks.</param>
        /// <returns>The result.</returns>
        public static DateTimeOffset AddWeeks(this DateTimeOffset value, int weeks)
            => value.AddDays(7 * weeks);

        /// <summary>
        /// Returns the last tick of the calendar day in the same offset as <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The date and time with offset.</param>
        /// <returns>The end of the day.</returns>
        public static DateTimeOffset EndOfDay(this DateTimeOffset value)
            => value.StartOfDay().AddDays(1).AddTicks(-1);

        /// <summary>
        /// Returns the last tick of the last day in the month of <paramref name="value"/>, in the same offset.
        /// </summary>
        /// <param name="value">The date and time with offset.</param>
        /// <returns>The end of the month.</returns>
        public static DateTimeOffset EndOfMonth(this DateTimeOffset value)
            => value.StartOfMonth().AddMonths(1).AddTicks(-1);

        /// <summary>
        /// Determines whether the date falls on Saturday or Sunday.
        /// </summary>
        /// <param name="value">The date and time with offset.</param>
        /// <returns><c>true</c> if the day is weekend; otherwise, <c>false</c>.</returns>
        public static bool IsWeekend(this DateTimeOffset value)
            => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        /// <summary>
        /// Returns midnight at the start of the calendar day with the same offset as <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The date and time with offset.</param>
        /// <returns>The start of the day.</returns>
        public static DateTimeOffset StartOfDay(this DateTimeOffset value)
            => new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, 0, value.Offset);

        /// <summary>
        /// Returns midnight at the first day of the month with the same offset as <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The date and time with offset.</param>
        /// <returns>The start of the month.</returns>
        public static DateTimeOffset StartOfMonth(this DateTimeOffset value)
            => new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, 0, value.Offset);

        #endregion Methods
    }
}
