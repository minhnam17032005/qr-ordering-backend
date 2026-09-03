using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.History;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class OrderItemHistoryConfiguration
        : IEntityTypeConfiguration<OrderItemHistory>
    {
        public void Configure(EntityTypeBuilder<OrderItemHistory> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("OrderItemHistories");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.Status)
                .IsRequired();


            // ============================================================
            // INDEXES
            // ============================================================

            // Tìm các item history của một OrderHistory
            builder.HasIndex(x => x.OrderHistoryId);

            // Tìm lịch sử theo Product gốc nếu có nhu cầu
            builder.HasIndex(x => x.ProductId);


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // OrderHistory 1 - N OrderItemHistory
            builder.HasOne(x => x.OrderHistory)
                .WithMany(x => x.OrderItemHistories)
                .HasForeignKey(x => x.OrderHistoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}