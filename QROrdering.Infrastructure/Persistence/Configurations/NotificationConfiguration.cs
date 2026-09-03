using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration
        : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("Notifications");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .IsRequired()
                .HasDefaultValue(false);


            // ============================================================
            // INDEXES
            // ============================================================

            // Lấy notification theo nhà hàng
            builder.HasIndex(x => x.RestaurantId);

            // Lấy notification của một User
            builder.HasIndex(x => x.UserId);

            // Lấy notification liên quan đến Order
            builder.HasIndex(x => x.OrderId);


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N Notification
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);


            // User 1 - N Notification
            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // Order 1 - N Notification
            // Cross-Tenant Protection:
            // Notification.RestaurantId + Notification.OrderId
            // phải cùng Restaurant với Order.
            builder.HasOne(x => x.Order)
                .WithMany(x => x.Notifications)
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