namespace Azoxia.Core.ValueTypes
{
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Represents a postal address with required locality fields and optional second line or postal code.
    /// </summary>
    public readonly record struct Address
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance with the specified components.
        /// </summary>
        /// <param name="line1">The primary address line.</param>
        /// <param name="city">The city or locality.</param>
        /// <param name="country">The country name or code.</param>
        /// <param name="line2">An optional second line (e.g. unit).</param>
        /// <param name="district">An optional district or region.</param>
        /// <param name="postalCode">An optional postal or ZIP code.</param>
        /// <exception cref="AzoxiaException">A required field is null or white-space.</exception>
        public Address(
            string line1,
            string city,
            string country,
            string? line2 = null,
            string? district = null,
            string? postalCode = null)
        {
            line1.ThrowIfNullOrWhiteSpace();
            city.ThrowIfNullOrWhiteSpace();
            country.ThrowIfNullOrWhiteSpace();

            Line1 = line1;
            City = city;
            Country = country;
            Line2 = line2;
            District = district;
            PostalCode = postalCode;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Gets the city or locality.
        /// </summary>
        public string City { get; }

        /// <summary>
        /// Gets the country name or code.
        /// </summary>
        public string Country { get; }

        /// <summary>
        /// Gets the optional district or region.
        /// </summary>
        public string? District { get; }

        /// <summary>
        /// Gets the primary address line.
        /// </summary>
        public string Line1 { get; }

        /// <summary>
        /// Gets the optional second address line.
        /// </summary>
        public string? Line2 { get; }

        /// <summary>
        /// Gets the optional postal or ZIP code.
        /// </summary>
        public string? PostalCode { get; }

        #endregion Properties
    }
}
