using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: Users
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: Restaurant 1 - N User
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: User 1 - N UserSession
            builder.HasMany(x => x.UserSessions)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: User N - N Role
            // Thông qua bảng trung gian UserRole
            builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: User 1 - N Notification
            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // Username must be unique within the same Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Username
            })
            .IsUnique();

            // Email must be unique within the same Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Email
            })
            .IsUnique();
        }
    }
}