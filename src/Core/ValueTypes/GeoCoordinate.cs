namespace Azoxia.Core.ValueTypes
{
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Represents a geographic point as latitude and longitude in decimal degrees (WGS84-style usage).
    /// </summary>
    public readonly record struct GeoCoordinate
    {
        #region Fields

        private const double EarthRadiusMetres = 6_371_000d;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Initializes a new instance with the specified coordinates.
        /// </summary>
        /// <param name="latitude">Degrees in the range [-90, 90].</param>
        /// <param name="longitude">Degrees in the range [-180, 180].</param>
        /// <exception cref="AzoxiaException"><paramref name="latitude"/> or <paramref name="longitude"/> is out of range.</exception>
        public GeoCoordinate(double latitude, double longitude)
        {
            latitude.ThrowIfOutOfRange(-90d, 90d, AzoxiaErrorCodes.GeoCoordinateLatitudeInvalid);
            longitude.ThrowIfOutOfRange(-180d, 180d, AzoxiaErrorCodes.GeoCoordinateLongitudeInvalid);

            Latitude = latitude;
            Longitude = longitude;
        }

        #endregion Ctors

        #region Utils

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180d);
        }

        #endregion Utils

        #region Methods

        /// <summary>
        /// Returns the great-circle distance to <paramref name="other"/> in metres (spherical Earth approximation).
        /// </summary>
        /// <param name="other">The other coordinate.</param>
        /// <returns>Distance in metres.</returns>
        public double DistanceTo(GeoCoordinate other)
        {
            double deltaLat = ToRadians(other.Latitude - Latitude);
            double deltaLon = ToRadians(other.Longitude - Longitude);

            double a = Math.Sin(deltaLat / 2)
                * Math.Sin(deltaLat / 2)
                + Math.Cos(ToRadians(Latitude))
                * Math.Cos(ToRadians(other.Latitude))
                * Math.Sin(deltaLon / 2)
                * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusMetres * c;
        }

        /// <summary>
        /// Indicates whether this instance matches <paramref name="other"/> within a small tolerance.
        /// </summary>
        /// <param name="other">The other coordinate.</param>
        /// <returns><c>true</c> if lat/lon are effectively equal.</returns>
        public bool Equals(GeoCoordinate other) =>
            Math.Abs(Latitude - other.Latitude) < 1e-9 &&
            Math.Abs(Longitude - other.Longitude) < 1e-9;

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(Math.Round(Latitude, 9), Math.Round(Longitude, 9));

        /// <summary>
        /// Determines whether this point lies within <paramref name="radiusMetres"/> of <paramref name="centre"/>.
        /// </summary>
        /// <param name="centre">The centre point.</param>
        /// <param name="radiusMetres">The inclusive radius in metres.</param>
        /// <returns><c>true</c> if this point is inside or on the boundary of the circle.</returns>
        public bool IsWithin(GeoCoordinate centre, double radiusMetres)
            => DistanceTo(centre) <= radiusMetres;

        /// <inheritdoc />
        public override string ToString()
            => $"({Latitude:F6}, {Longitude:F6})";

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the latitude in decimal degrees ([-90, 90]).
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets the longitude in decimal degrees ([-180, 180]).
        /// </summary>
        public double Longitude { get; }

        #endregion Properties
    }
}
