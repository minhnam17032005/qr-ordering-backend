using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: OrderHistories
    public class OrderHistoryConfiguration : IEntityTypeConfiguration<OrderHistory>
    {
        public void Configure(EntityTypeBuilder<OrderHistory> builder)
        {
            builder.ToTable("OrderHistories");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: Restaurant 1 - N OrderHistory
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.OrderHistories)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: OrderHistory 1 - N OrderItemHistory
            builder.HasMany(x => x.OrderItemHistories)
                .WithOne(x => x.OrderHistory)
                .HasForeignKey(x => x.OrderHistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Index / Unique
            // =========================

            // Query order history by Restaurant
            builder.HasIndex(x => x.RestaurantId);

            // Query history by Order
            builder.HasIndex(x => x.OrderId);

            // Query history by Restaurant + OrderCode
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.OrderCode
            });
        }
    }
}