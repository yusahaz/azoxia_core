namespace Azoxia.Core.ValueTypes
{
    using System;
    using System.Globalization;

    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Represents a monetary amount with an associated currency symbol or code string (culture-dependent for the parameterless currency overload).
    /// </summary>
    public readonly record struct Money
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance with <paramref name="amount"/> and the current culture's currency symbol.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        public Money(decimal amount)
            : this(amount, CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol)
        {
        }

        /// <summary>
        /// Initializes a new instance with <paramref name="amount"/> and <paramref name="currency"/> (symbol or ISO code, depending on caller).
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currency">Non-empty currency symbol or identifier.</param>
        /// <exception cref="AzoxiaException"><paramref name="currency"/> is null or white-space.</exception>
        public Money(decimal amount, string currency)
        {
            currency.ThrowIfNullOrWhiteSpace();

            Amount = amount;
            Currency = currency;
        }

        #endregion Ctors

        #region Utils

        private static void ThrowIfCurrencyMismatch(Money left, Money right)
        {
            string.Equals(left.Currency, right.Currency, StringComparison.OrdinalIgnoreCase)
                .ThrowIfFalse(AzoxiaErrorCodes.MoneyCurrencyMismatch);
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Gets the monetary amount represented by this instance.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Gets the currency symbol or code associated with this amount (as supplied at construction).
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Gets a value indicating whether the amount is zero.
        /// </summary>
        public bool IsZero => Amount == decimal.Zero;

        #endregion Properties

        #region Operators

        /// <summary>
        /// Adds two <see cref="Money"/> values that have the same currency.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>The sum in the shared currency.</returns>
        /// <exception cref="AzoxiaException">Currencies do not match.</exception>
        public static Money operator +(Money left, Money right)
        {
            ThrowIfCurrencyMismatch(left, right);
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        /// <summary>
        /// Divides <paramref name="money"/> by <paramref name="divisor"/>.
        /// </summary>
        /// <param name="money">The dividend.</param>
        /// <param name="divisor">A value greater than zero.</param>
        /// <returns>The quotient in the same currency.</returns>
        /// <exception cref="AzoxiaException"><paramref name="divisor"/> is not greater than zero.</exception>
        public static Money operator /(Money money, decimal divisor)
        {
            (divisor > 0).ThrowIfFalse(AzoxiaErrorCodes.MoneyDivisorInvalid);
            return new Money(money.Amount / divisor, money.Currency);
        }

        /// <summary>
        /// Determines whether <paramref name="left"/> is strictly greater than <paramref name="right"/> (same currency).
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns><c>true</c> if <paramref name="left"/> is greater; otherwise <c>false</c>.</returns>
        /// <exception cref="AzoxiaException">Currencies do not match.</exception>
        public static bool operator >(Money left, Money right)
        {
            ThrowIfCurrencyMismatch(left, right);
            return left.Amount > right.Amount;
        }

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than or equal to <paramref name="right"/> (same currency).
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns><c>true</c> if <paramref name="left"/> is greater or equal; otherwise <c>false</c>.</returns>
        /// <exception cref="AzoxiaException">Currencies do not match.</exception>
        public static bool operator >=(Money left, Money right)
        {
            ThrowIfCurrencyMismatch(left, right);
            return left.Amount >= right.Amount;
        }

        /// <summary>
        /// Determines whether <paramref name="left"/> is strictly less than <paramref name="right"/> (same currency).
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns><c>true</c> if <paramref name="left"/> is less; otherwise <c>false</c>.</returns>
        /// <exception cref="AzoxiaException">Currencies do not match.</exception>
        public static bool operator <(Money left, Money right)
        {
            ThrowIfCurrencyMismatch(left, right);
            return left.Amount < right.Amount;
        }

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than or equal to <paramref name="right"/> (same currency).
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns><c>true</c> if <paramref name="left"/> is less or equal; otherwise <c>false</c>.</returns>
        /// <exception cref="AzoxiaException">Currencies do not match.</exception>
        public static bool operator <=(Money left, Money right)
        {
            ThrowIfCurrencyMismatch(left, right);
            return left.Amount <= right.Amount;
        }

        /// <summary>
        /// Multiplies <paramref name="money"/> by <paramref name="factor"/>.
        /// </summary>
        /// <param name="money">The monetary value.</param>
        /// <param name="factor">A non-negative scale factor.</param>
        /// <returns>The product in the same currency.</returns>
        /// <exception cref="AzoxiaException"><paramref name="factor"/> is negative.</exception>
        public static Money operator *(Money money, decimal factor)
        {
            (factor >= 0).ThrowIfFalse(AzoxiaErrorCodes.MoneyFactorNegative);
            return new Money(money.Amount * factor, money.Currency);
        }

        /// <summary>
        /// Subtracts <paramref name="right"/> from <paramref name="left"/> (same currency, non-negative result).
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The subtrahend.</param>
        /// <returns>The difference in the shared currency.</returns>
        /// <exception cref="AzoxiaException">Currencies do not match or the result would be negative.</exception>
        public static Money operator -(Money left, Money right)
        {
            ThrowIfCurrencyMismatch(left, right);
            decimal amount = left.Amount - right.Amount;
            (amount >= 0).ThrowIfFalse(AzoxiaErrorCodes.MoneySubtractionNegative);
            return new Money(amount, left.Currency);
        }

        #endregion Operators
    }
}
