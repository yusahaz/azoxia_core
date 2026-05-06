namespace Azoxia.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DateOnly"/>.
    /// </summary>
    public static class DateOnlyExtensions
    {
        #region Methods

        /// <summary>
        /// Adds the specified number of weeks to the value.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <param name="weeks">The number of weeks.</param>
        /// <returns>The result.</returns>
        public static DateOnly AddWeeks(this DateOnly value, int weeks)
            => value.AddDays(7 * weeks);

        /// <summary>
        /// Returns the last calendar day of the month for <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <returns>The last day of the month.</returns>
        public static DateOnly EndOfMonth(this DateOnly value)
            => new DateOnly(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));

        /// <summary>
        /// Determines whether the date falls on Saturday or Sunday.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <returns><c>true</c> if the day is weekend; otherwise, <c>false</c>.</returns>
        public static bool IsWeekend(this DateOnly value)
            => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        /// <summary>
        /// Returns the first calendar day of the month for <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <returns>The first day of the month.</returns>
        public static DateOnly StartOfMonth(this DateOnly value)
            => new DateOnly(value.Year, value.Month, 1);

        #endregion Methods
    }
}
