namespace Azoxia.Core.Persistence.Mapping
{
    using Azoxia.Core.ValueTypes;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Shared EF complex-type mappings for Core value types and single-string <c>Value</c> complex shapes.
    /// Use relational <c>HasColumnName</c> only here (nested under <c>ComplexProperty</c>); do not use <c>HasColumnOrder</c> inside these blocks.
    /// </summary>
    public static class ValueTypeComplexMapping
    {
        #region Methods

        /// <summary>
        /// Maps <see cref="Address"/> columns with the given prefix.
        /// </summary>
        public static void MapAddress(ComplexPropertyBuilder<Address> builder, string prefix)
        {
            builder.Property(a => a.Line1).HasColumnName($"{prefix}_Line1").HasMaxLength(512).IsRequired();
            builder.Property(a => a.City).HasColumnName($"{prefix}_City").HasMaxLength(256).IsRequired();
            builder.Property(a => a.Country).HasColumnName($"{prefix}_Country").HasMaxLength(128).IsRequired();
            builder.Property(a => a.Line2).HasColumnName($"{prefix}_Line2").HasMaxLength(512).IsRequired(false);
            builder.Property(a => a.District).HasColumnName($"{prefix}_District").HasMaxLength(256).IsRequired(false);
            builder.Property(a => a.PostalCode).HasColumnName($"{prefix}_PostalCode").HasMaxLength(32).IsRequired(false);
        }

        /// <summary>
        /// Maps <see cref="Contact"/> columns with the given prefix.
        /// </summary>
        public static void MapContact(ComplexPropertyBuilder<Contact> builder, string prefix)
        {
            builder.Property(c => c.FirstName).HasColumnName($"{prefix}_FirstName").HasMaxLength(256).IsRequired();
            builder.Property(c => c.LastName).HasColumnName($"{prefix}_LastName").HasMaxLength(256).IsRequired();
            builder.Property(c => c.Email).HasColumnName($"{prefix}_Email").HasMaxLength(512).IsRequired();
            builder.Property(c => c.Phone).HasColumnName($"{prefix}_Phone").HasMaxLength(64).IsRequired();
        }

        /// <summary>
        /// Maps <see cref="GeoCoordinate"/> columns with the given prefix.
        /// </summary>
        public static void MapGeoCoordinate(ComplexPropertyBuilder<GeoCoordinate> builder, string prefix)
        {
            builder.Property(g => g.Latitude).HasColumnName($"{prefix}_Latitude").IsRequired();
            builder.Property(g => g.Longitude).HasColumnName($"{prefix}_Longitude").IsRequired();
        }

        /// <summary>
        /// Maps <see cref="Money"/> amount and currency columns.
        /// </summary>
        public static void MapMoney(ComplexPropertyBuilder<Money> builder, string amountColumn, string currencyColumn)
        {
            builder.Property(m => m.Amount).HasColumnName(amountColumn).HasPrecision(18, 2).IsRequired();
            builder.Property(m => m.Currency).HasColumnName(currencyColumn).HasMaxLength(16).IsRequired();
        }

        /// <summary>
        /// Maps a complex CLR type that exposes a single required string property named <c>Value</c> to one column (domain value objects such as tax numbers or tags).
        /// </summary>
        /// <typeparam name="TComplex">The complex type.</typeparam>
        /// <param name="builder">The complex property builder.</param>
        /// <param name="columnName">The relational column name.</param>
        /// <param name="maxLength">Maximum string length.</param>
        public static void MapStringValueColumn<TComplex>(ComplexPropertyBuilder<TComplex> builder, string columnName, int maxLength)
            where TComplex : struct
        {
            builder.Property("Value")
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();
        }

        #endregion Methods
    }
}
