using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: Orders
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // =========================
            // Relationships
            // =========================

            // Restaurant 1 - N Order
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // CustomerSession 1 - N Order
            builder.HasOne(x => x.CustomerSession)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 1 - N OrderItem
            builder.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 1 - N Payment
            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 1 - N Notification
            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // OrderCode is unique within Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.OrderCode
            })
            .IsUnique();

            // Query orders by Restaurant + Status
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Status
            });

            // Query orders by CustomerSession
            builder.HasIndex(x => x.CustomerSessionId);
        }
    }
}