namespace Azoxia.Core.ValueTypes
{
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Represents a person's basic contact details for display or messaging.
    /// </summary>
    public readonly record struct Contact
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance with the specified non-empty fields.
        /// </summary>
        /// <param name="firstName">The given name.</param>
        /// <param name="lastName">The family name.</param>
        /// <param name="email">The email address.</param>
        /// <param name="phone">The phone number.</param>
        /// <exception cref="AzoxiaException">Any required field is null or white-space.</exception>
        public Contact(
            string firstName,
            string lastName,
            string email,
            string phone)
        {
            firstName.ThrowIfNullOrWhiteSpace();
            lastName.ThrowIfNullOrWhiteSpace();
            email.ThrowIfNullOrWhiteSpace();
            phone.ThrowIfNullOrWhiteSpace();

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Gets the given name.
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// Gets the family name.
        /// </summary>
        public string LastName { get; }

        /// <summary>
        /// Gets the phone number.
        /// </summary>
        public string Phone { get; }

        #endregion Properties
    }
}
