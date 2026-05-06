namespace Azoxia.Core.Persistence.DbLogging
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Maps <see cref="AppLog"/> to the <c>AppLogs</c> table.
    /// </summary>
    public sealed class AppLogConfiguration :
        IEntityTypeConfiguration<AppLog>
    {
        #region Methods

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<AppLog> builder)
        {
            builder.ToTable("AppLogs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.UtcTimestamp)
                .IsRequired();

            builder.Property(e => e.Level)
                .IsRequired();

            builder.Property(e => e.Category)
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(e => e.Message)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(e => e.Exception)
                .HasMaxLength(8000);

            builder.Property(e => e.EventName)
                .HasMaxLength(256);

            builder.HasIndex(e => e.UtcTimestamp);
        }

        #endregion Methods
    }
}
