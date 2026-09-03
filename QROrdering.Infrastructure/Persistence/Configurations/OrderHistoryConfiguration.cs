using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.History;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class OrderHistoryConfiguration
        : IEntityTypeConfiguration<OrderHistory>
    {
        public void Configure(EntityTypeBuilder<OrderHistory> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("OrderHistories");


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

            builder.Property(x => x.TableNumber)
                .IsRequired();

            builder.Property(x => x.OrderStatus)
                .IsRequired();

            builder.Property(x => x.PaymentMethod)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            // ============================================================
            // INDEXES
            // ============================================================

            // Tìm toàn bộ lịch sử của một Order
            builder.HasIndex(x => x.OrderId);

            // Query lịch sử theo Restaurant + thời gian
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.CreatedAt
            });

            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N OrderHistory
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.OrderHistories)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Order 1 - N OrderHistory
            // Cross-Tenant Protection:
            // OrderHistory.RestaurantId + OrderHistory.OrderId
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
        }
    }
}