using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Authorization;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration
        : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("Roles");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // ============================================================
            // ALTERNATE KEY
            // ============================================================

            // Principal key dùng cho Composite FK chống Cross-Tenant
            builder.HasAlternateKey(x => new
            {
                x.RestaurantId,
                x.Id
            });

            // ============================================================
            // INDEXES
            // ============================================================

            // Một nhà hàng không nên có 2 Role cùng tên
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Name
            })
            .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N Role
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}