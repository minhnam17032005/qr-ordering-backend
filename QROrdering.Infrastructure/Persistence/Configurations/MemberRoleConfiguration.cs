using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class MemberRoleConfiguration
        : IEntityTypeConfiguration<MemberRole>
    {
        public void Configure(EntityTypeBuilder<MemberRole> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("MemberRoles");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // INDEXES
            // ============================================================

            // Một RestaurantMember không được gán cùng một Role nhiều lần
            builder.HasIndex(x => new
            {
                x.RestaurantMemberId,
                x.RoleId
            })
            .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // RestaurantMember 1 - N MemberRole
            builder.HasOne(x => x.RestaurantMember)
                .WithMany(x => x.MemberRoles)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.RestaurantMemberId
                })
                .HasPrincipalKey(x => new
                {
                    x.RestaurantId,
                    x.Id
                })
                .OnDelete(DeleteBehavior.Cascade);


            // Role 1 - N MemberRole
            builder.HasOne(x => x.Role)
                .WithMany(x => x.MemberRoles)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.RoleId
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