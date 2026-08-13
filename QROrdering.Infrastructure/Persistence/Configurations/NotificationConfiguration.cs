using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: Notifications
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // =========================
            // Relationships
            // =========================

            // Restaurant 1 - N Notification
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 1 - N Notification
            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 1 - N Notification
            builder.HasOne(x => x.Order)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index
            // =========================

            // Query notifications by Restaurant
            builder.HasIndex(x => x.RestaurantId);

            // Query notifications of a User
            builder.HasIndex(x => x.UserId);

            // Query notifications related to an Order
            builder.HasIndex(x => x.OrderId);

            // Query unread notifications of a User
            builder.HasIndex(x => new
            {
                x.UserId,
                x.IsRead
            });

        }
    }
}