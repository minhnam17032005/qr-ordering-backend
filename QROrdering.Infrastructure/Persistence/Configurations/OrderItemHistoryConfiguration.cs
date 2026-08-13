using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: OrderItemHistories
    public class OrderItemHistoryConfiguration : IEntityTypeConfiguration<OrderItemHistory>
    {
        public void Configure(EntityTypeBuilder<OrderItemHistory> builder)
        {
            builder.ToTable("OrderItemHistories");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: OrderHistory 1 - N OrderItemHistory
            builder.HasOne(x => x.OrderHistory)
                .WithMany(x => x.OrderItemHistories)
                .HasForeignKey(x => x.OrderHistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Index
            // =========================

            // Query items by OrderHistory
            builder.HasIndex(x => x.OrderHistoryId);

            // Query history items by Product
            builder.HasIndex(x => x.ProductId);
        }
    }
}