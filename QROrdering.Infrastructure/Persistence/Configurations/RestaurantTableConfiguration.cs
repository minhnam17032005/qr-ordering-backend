using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class RestaurantTableConfiguration
        : IEntityTypeConfiguration<RestaurantTable>
    {
        public void Configure(EntityTypeBuilder<RestaurantTable> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("RestaurantTables");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.TableNumber)
                .IsRequired();

            builder.Property(x => x.QRCode)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // ============================================================
            // ALTERNATE KEY
            // ============================================================

            // Principal key dùng cho Composite FK chống Cross-Tenant
            builder.HasAlternateKey(x => new
            {
                x.RestaurantId,
                x.Id
            });

            // ============================================================
            // INDEXES
            // ============================================================

            // TableNumber unique trong phạm vi Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.TableNumber
            })
            .IsUnique();

            // QRCode unique
            builder.HasIndex(x => x.QRCode)
                .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N RestaurantTable
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.RestaurantTables)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}