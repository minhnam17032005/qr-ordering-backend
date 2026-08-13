using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: OrderItems
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: Order 1 - N OrderItem
            builder.HasOne(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Product 1 - N OrderItem
            builder.HasOne(x => x.Product)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index
            // =========================

            // Query items by Order
            builder.HasIndex(x => x.OrderId);

            // Query items by Product
            builder.HasIndex(x => x.ProductId);

            // Query items by Order + Status
            builder.HasIndex(x => new
            {
                x.OrderId,
                x.Status
            });
        }
    }
}