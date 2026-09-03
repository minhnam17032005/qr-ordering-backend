using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class PlatformAdminConfiguration
        : IEntityTypeConfiguration<PlatformAdmin>
    {
        public void Configure(EntityTypeBuilder<PlatformAdmin> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("PlatformAdmins");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(x => x.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);


            // ============================================================
            // INDEXES
            // ============================================================

            builder.HasIndex(x => x.Username)
                .IsUnique();

            builder.HasIndex(x => x.Email)
                .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================
        }
    }
}