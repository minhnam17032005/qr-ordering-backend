using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration
        : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("Orders");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.OrderCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Note)
                .HasMaxLength(500);

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

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

            // Tìm Order theo Restaurant
            builder.HasIndex(x => x.RestaurantId);

            // Tìm Order theo CustomerSession
            builder.HasIndex(x => x.CustomerSessionId);

            // OrderCode phải unique trong phạm vi Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.OrderCode
            })
            .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N Order
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);


            // CustomerSession 1 - N Order
            // Cross-Tenant Protection:
            // Order.RestaurantId + Order.CustomerSessionId
            // phải cùng Restaurant với CustomerSession.
            builder.HasOne(x => x.CustomerSession)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.CustomerSessionId
                })
                .HasPrincipalKey(x => new
                {
                    x.RestaurantId,
                    x.Id
                })
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}