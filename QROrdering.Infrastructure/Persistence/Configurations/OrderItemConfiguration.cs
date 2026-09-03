using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration
        : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("OrderItems");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.Note)
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .IsRequired();


            // ============================================================
            // INDEXES
            // ============================================================

            // Lấy OrderItems theo Order
            builder.HasIndex(x => x.OrderId);

            // Lấy OrderItems theo Product
            builder.HasIndex(x => x.ProductId);

            // Hỗ trợ tenant-scoped queries
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.OrderId
            });


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Order 1 - N OrderItem
            // Cross-Tenant Protection:
            // OrderItem.RestaurantId + OrderItem.OrderId
            // phải cùng Restaurant với Order.
            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.OrderId
                })
                .HasPrincipalKey(x => new
                {
                    x.RestaurantId,
                    x.Id
                })
                .OnDelete(DeleteBehavior.NoAction);


            // Product 1 - N OrderItem
            // Cross-Tenant Protection:
            // OrderItem.RestaurantId + OrderItem.ProductId
            // phải cùng Restaurant với Product.
            builder.HasOne(x => x.Product)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.ProductId
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