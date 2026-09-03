using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class RestaurantMemberConfiguration
        : IEntityTypeConfiguration<RestaurantMember>
    {
        public void Configure(EntityTypeBuilder<RestaurantMember> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("RestaurantMembers");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

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

            // Một User chỉ có một membership trong một Restaurant
            builder.HasIndex(x => new
            {
                x.UserId,
                x.RestaurantId
            })
            .IsUnique();


                // ============================================================
                // RELATIONSHIPS
                // ============================================================

                // User 1 - N RestaurantMember
                builder.HasOne(x => x.User)
                    .WithMany(x => x.RestaurantMembers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);


                // Restaurant 1 - N RestaurantMember
                builder.HasOne(x => x.Restaurant)
                    .WithMany(x => x.RestaurantMembers)
                    .HasForeignKey(x => x.RestaurantId)
                    .OnDelete(DeleteBehavior.NoAction);

        }
    }
}