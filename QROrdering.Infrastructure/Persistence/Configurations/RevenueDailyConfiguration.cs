using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: RevenueDaily
    public class RevenueDailyConfiguration : IEntityTypeConfiguration<RevenueDaily>
    {
        public void Configure(EntityTypeBuilder<RevenueDaily> builder)
        {
            builder.ToTable("RevenueDaily");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // Relationship: Restaurant 1 - N RevenueDaily
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.RevenueDailies)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // One revenue record per Restaurant per day
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.RevenueDate
            })
            .IsUnique();
        }
    }
}