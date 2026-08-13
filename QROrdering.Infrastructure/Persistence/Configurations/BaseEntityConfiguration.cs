using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public static class BaseEntityConfiguration
    {
        public static void ConfigureBaseEntity<T>(
            this EntityTypeBuilder<T> builder)
            where T : BaseEntity
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Id
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // CreatedAt
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // UpdatedAt
            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);
        }
    }
}