using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("Payments");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.PaymentMethod)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.TransactionCode)
                .HasMaxLength(100);

            builder.Property(x => x.PaidAt);


            // ============================================================
            // INDEXES
            // ============================================================

            // Lấy Payment theo Order
            builder.HasIndex(x => x.OrderId);

            // Tìm giao dịch theo TransactionCode
            builder.HasIndex(x => x.TransactionCode)
                .IsUnique()
                .HasFilter("[TransactionCode] IS NOT NULL");


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Order 1 - N Payment
            builder.HasOne(x => x.Order)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}