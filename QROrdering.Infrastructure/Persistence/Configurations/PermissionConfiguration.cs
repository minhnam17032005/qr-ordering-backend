using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Authorization;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration
        : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            // Table
            builder.ToTable("Permissions");

            // Base Entity
            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.ApiPath)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Method)
                .IsRequired();

            builder.Property(x => x.Module)
                .IsRequired()
                .HasMaxLength(100);


            // ============================================================
            // INDEXES
            // ============================================================

            builder.HasIndex(x => new
            {
                x.ApiPath,
                x.Method
            })
            .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================
        }
    }
}