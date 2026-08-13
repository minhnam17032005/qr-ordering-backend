using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: Roles
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: Restaurant 1 - N Role
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Role N - N User
            // Thông qua bảng trung gian UserRole
            builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Role N - N Permission
            // Thông qua bảng trung gian RolePermission
            builder.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Index / Unique
            // =========================

            // Role name must be unique within the same Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Name
            })
            .IsUnique();
        }
    }
}