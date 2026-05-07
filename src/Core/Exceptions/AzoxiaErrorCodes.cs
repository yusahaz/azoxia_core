namespace Azoxia.Core.Exceptions
{
    /// <summary>
    /// Fixed <see cref="ErrorCode"/> values for the Core layer.
    /// Error text and codes are defined only in this type; other code references these members.
    /// </summary>
    public static class AzoxiaErrorCodes
    {
        #region Properties

        /// <summary>Reference-type argument is null.</summary>
        public static readonly ErrorCode ArgumentNull = new(
            Code: "AZX_CORE_001",
            ErrorMessage: "The value cannot be null.");

        /// <summary>Value is outside the allowed range.</summary>
        public static readonly ErrorCode ArgumentOutOfRange = new(
            Code: "AZX_CORE_006",
            ErrorMessage: "The value is not in the allowed range.");

        /// <summary>Condition was expected to be false.</summary>
        public static readonly ErrorCode ExpectedConditionFalse = new(
            Code: "AZX_CORE_005",
            ErrorMessage: "Invalid state: the condition was expected to be false.");

        /// <summary>Condition was expected to be true.</summary>
        public static readonly ErrorCode ExpectedConditionTrue = new(
            Code: "AZX_CORE_004",
            ErrorMessage: "The expected condition was not met.");

        /// <summary>Latitude is not between -90 and 90 degrees inclusive.</summary>
        public static readonly ErrorCode GeoCoordinateLatitudeInvalid = new(
            Code: "AZX_CORE_012",
            ErrorMessage: "Latitude must be between -90 and 90 degrees.");

        /// <summary>Longitude is not between -180 and 180 degrees inclusive.</summary>
        public static readonly ErrorCode GeoCoordinateLongitudeInvalid = new(
            Code: "AZX_CORE_013",
            ErrorMessage: "Longitude must be between -180 and 180 degrees.");

        /// <summary><c>JwtConfig:Key</c> is shorter than the minimum length for symmetric signing.</summary>
        public static readonly ErrorCode JwtConfigSigningKeyTooShort = new(
            Code: "AZX_CORE_022",
            ErrorMessage: "JwtConfig:Key must be at least 32 characters.");

        /// <summary>Operation is not valid in the current state.</summary>
        public static readonly ErrorCode InvalidOperation = new(
            Code: "AZX_CORE_007",
            ErrorMessage: "The operation is not valid for the current state.");

        /// <summary>Monetary operands use different currencies.</summary>
        public static readonly ErrorCode MoneyCurrencyMismatch = new(
            Code: "AZX_CORE_014",
            ErrorMessage: "Both monetary values must use the same currency.");

        /// <summary>Monetary division divisor is not greater than zero.</summary>
        public static readonly ErrorCode MoneyDivisorInvalid = new(
            Code: "AZX_CORE_015",
            ErrorMessage: "The divisor must be greater than zero.");

        /// <summary>Monetary multiplication factor is negative.</summary>
        public static readonly ErrorCode MoneyFactorNegative = new(
            Code: "AZX_CORE_016",
            ErrorMessage: "The factor cannot be negative.");

        /// <summary>Monetary subtraction would yield a negative amount.</summary>
        public static readonly ErrorCode MoneySubtractionNegative = new(
            Code: "AZX_CORE_017",
            ErrorMessage: "The resulting amount cannot be negative.");

        /// <summary>Requested key or entity was not found.</summary>
        public static readonly ErrorCode NotFound = new(
            Code: "AZX_CORE_008",
            ErrorMessage: "The requested record or key was not found.");

        /// <summary>User is not allowed to perform the requested operation.</summary>
        public static readonly ErrorCode PermissionDenied = new(
            Code: "AZX_CORE_041",
            ErrorMessage: "You are not allowed to perform this operation.");

        /// <summary>Missing or invalid system_user_id claim required for permission evaluation.</summary>
        public static readonly ErrorCode SystemUserIdClaimRequired = new(
            Code: "AZX_CORE_042",
            ErrorMessage: "A valid system_user_id claim is required.");

        /// <summary>Nullable value type has no value.</summary>
        public static readonly ErrorCode NullableValueMissing = new(
            Code: "AZX_CORE_009",
            ErrorMessage: "A value was expected but none was present.");

        /// <summary>Concurrent modification detected while saving persistence changes.</summary>
        public static readonly ErrorCode PersistenceConcurrencyConflict = new(
            Code: "AZX_CORE_018",
            ErrorMessage: "The data could not be saved because another user modified it concurrently.");

        /// <summary>Persistence layer rejected an update (constraint violation or provider-specific failure).</summary>
        public static readonly ErrorCode PersistenceSaveFailed = new(
            Code: "AZX_CORE_019",
            ErrorMessage: "The data could not be saved due to a database constraint or validation error.");

        /// <summary>Database operation exceeded the allowed time.</summary>
        public static readonly ErrorCode PersistenceTimeout = new(
            Code: "AZX_CORE_020",
            ErrorMessage: "The database operation timed out.");

        /// <summary>Request validation failed in application pipeline.</summary>
        public static readonly ErrorCode RequestValidationFailed = new(
            Code: "AZX_CORE_021",
            ErrorMessage: "The request payload failed validation.");

        /// <summary>String split separator is null or empty.</summary>
        public static readonly ErrorCode SplitSeparatorInvalid = new(
            Code: "AZX_CORE_010",
            ErrorMessage: "The split separator cannot be null or empty.");

        /// <summary>String is null or empty.</summary>
        public static readonly ErrorCode StringNullOrEmpty = new(
            Code: "AZX_CORE_002",
            ErrorMessage: "The string cannot be null or empty.");

        /// <summary>String is null, empty, or white-space only.</summary>
        public static readonly ErrorCode StringNullOrWhiteSpace = new(
            Code: "AZX_CORE_003",
            ErrorMessage: "The string cannot be null, empty, or consist only of white-space characters.");

        /// <summary>Unix time is outside the range supported for conversion.</summary>
        public static readonly ErrorCode UnixTimeOutOfRange = new(
            Code: "AZX_CORE_011",
            ErrorMessage: "The Unix time value is outside the range supported for this conversion.");

        #endregion Properties
    }
}
