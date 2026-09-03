using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Authorization;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration
        : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("RolePermissions");


            // ============================================================
            // PRIMARY KEY
            // ============================================================

            builder.HasKey(x => new
            {
                x.RoleId,
                x.PermissionId
            });

            // ============================================================
            // INDEXES
            // ============================================================

            // Một Role không được gán cùng một Permission nhiều lần
            builder.HasIndex(x => new
            {
                x.RoleId,
                x.PermissionId
            })
            .IsUnique();

            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Role 1 - N RolePermission
            builder.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);


            // Permission 1 - N RolePermission
            builder.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}