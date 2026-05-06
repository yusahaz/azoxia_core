namespace Azoxia.Core.Persistence.Mapping
{
    using Azoxia.Core.Domain;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Opinionated EF Core mapping for entities deriving from <see cref="EntityBase"/>, including audits and soft delete fields when applicable.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public abstract class EntityTypeConfigurationBase<TEntity> :
        IEntityTypeConfiguration<TEntity>,
        IEntityTypeConfiguration
        where TEntity : class, IEntity
    {
        #region Utils

        /// <summary>
        /// Applies entity-specific column configuration after shared conventions run.
        /// </summary>
        /// <param name="builder">The entity builder.</param>
        /// <param name="columnOrder">Running column order counter.</param>
        protected internal virtual void Configure(EntityTypeBuilder<TEntity> builder, ref int columnOrder)
        {
        }

        #endregion Utils

        #region IEntityTypeConfiguration Members

        /// <inheritdoc />
        void IEntityTypeConfiguration<TEntity>.Configure(EntityTypeBuilder<TEntity> builder)
        {
            Type entityType = typeof(TEntity);

            builder.ToTable(entityType.Name);

            int columnOrder = 0;

            builder.Property(nameof(EntityBase.Id))
                .HasColumnOrder(columnOrder++);

            builder.HasKey(nameof(EntityBase.Id));

            if (typeof(ISupportedRowVersion).IsAssignableFrom(entityType))
            {
                builder.Property(nameof(ISupportedRowVersion.RowVersion))
                    .HasColumnOrder(columnOrder++)
                    .IsRowVersion()
                    .IsRequired();
            }

            if (typeof(CodedNamedEntityBase).IsAssignableFrom(entityType))
            {
                builder.Property(nameof(CodedNamedEntityBase.Name))
                    .HasMaxLength(1024)
                    .HasColumnOrder(columnOrder++)
                    .IsRequired();

                builder.Property(nameof(CodedNamedEntityBase.Description))
                    .HasColumnOrder(columnOrder++)
                    .IsRequired(false);

                builder.HasIndex(nameof(CodedNamedEntityBase.Name));
            }

            Configure(builder, ref columnOrder);

            if (typeof(AuditableEntityBase).IsAssignableFrom(entityType))
            {
                builder.Property(nameof(AuditableEntityBase.CreatedAt))
                    .HasColumnOrder(columnOrder++)
                    .IsRequired();

                builder.Property(nameof(AuditableEntityBase.CreatedBy))
                    .HasColumnOrder(columnOrder++)
                    .HasMaxLength(1024)
                    .IsRequired(false);

                builder.Property(nameof(AuditableEntityBase.UpdatedAt))
                    .HasColumnOrder(columnOrder++)
                    .IsRequired(false);

                builder.Property(nameof(AuditableEntityBase.UpdatedBy))
                    .HasColumnOrder(columnOrder++)
                    .HasMaxLength(1024)
                    .IsRequired(false);

                builder.HasIndex(nameof(AuditableEntityBase.CreatedAt));
            }

            if (typeof(DeletableEntityBase).IsAssignableFrom(entityType))
            {
                builder.Property(nameof(DeletableEntityBase.IsDeleted))
                    .HasColumnOrder(columnOrder++)
                    .IsRequired();

                builder.Property(nameof(DeletableEntityBase.DeletedAt))
                    .HasColumnOrder(columnOrder++)
                    .IsRequired(false);

                builder.Property(nameof(DeletableEntityBase.DeletedBy))
                    .HasColumnOrder(columnOrder++)
                    .HasMaxLength(1024)
                    .IsRequired(false);
            }
        }

        #endregion IEntityTypeConfiguration Members
    }
}
