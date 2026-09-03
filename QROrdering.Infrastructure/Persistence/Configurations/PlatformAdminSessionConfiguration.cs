using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class PlatformAdminSessionConfiguration
        : IEntityTypeConfiguration<PlatformAdminSession>
    {
        public void Configure(EntityTypeBuilder<PlatformAdminSession> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("PlatformAdminSessions");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.RefreshTokenHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.LastActivityAt)
                .IsRequired();

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);


            // ============================================================
            // INDEXES
            // ============================================================

            builder.HasIndex(x => x.PlatformAdminId);

            builder.HasIndex(x => x.RefreshTokenHash)
                .IsUnique();

            builder.HasIndex(x => x.ExpiresAt);


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // PlatformAdmin 1 - N PlatformAdminSession
            builder.HasOne(x => x.PlatformAdmin)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.PlatformAdminId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}