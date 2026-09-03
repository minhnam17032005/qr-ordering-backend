using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class RevenueDailyConfiguration
        : IEntityTypeConfiguration<RevenueDaily>
    {
        public void Configure(EntityTypeBuilder<RevenueDaily> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("RevenueDaily");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.RevenueDate)
                .IsRequired();

            builder.Property(x => x.TotalOrders)
                .IsRequired();

            builder.Property(x => x.CompletedOrders)
                .IsRequired();

            builder.Property(x => x.CancelledOrders)
                .IsRequired();

            builder.Property(x => x.TotalItemsSold)
                .IsRequired();

            builder.Property(x => x.TotalRevenue)
                .IsRequired()
                .HasPrecision(18, 2);


            // ============================================================
            // INDEXES
            // ============================================================

            // Mỗi Restaurant chỉ có một bản ghi doanh thu cho mỗi ngày
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.RevenueDate
            })
            .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N RevenueDaily
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.RevenueDailies)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}