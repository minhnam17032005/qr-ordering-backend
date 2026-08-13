using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: UserSessions
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: User 1 - N UserSession
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserSessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Index / Unique
            // =========================

            // Fast lookup when validating user's sessions
            builder.HasIndex(x => x.UserId);

            // RefreshTokenHash must be unique
            builder.HasIndex(x => x.RefreshTokenHash)
                .IsUnique();

            // Useful for finding active/expired sessions
            builder.HasIndex(x => new
            {
                x.UserId,
                x.ExpiredAt
            });
        }
    }
}