
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

            builder.Property(x => x.PaidAt)
                .IsRequired(false);


            // ============================================================
            // ALTERNATE KEY
            // ============================================================

            /*// Principal key dùng cho Composite FK chống Cross-Tenant
            builder.HasAlternateKey(x => new
            {
                x.RestaurantId,
                x.Id
            });*/


            // ============================================================
            // INDEXES
            // ============================================================

            // Tìm Payment theo Restaurant
            builder.HasIndex(x => x.RestaurantId);

            // Tìm Payment theo CustomerSession
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.CustomerSessionId
            });

            // Tìm giao dịch theo TransactionCode
            builder.HasIndex(x => x.TransactionCode)
                .IsUnique()
                .HasFilter("[TransactionCode] IS NOT NULL");


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N Payment
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);


            // CustomerSession 1 - N Payment
            // Cross-Tenant Protection:
            // Payment.RestaurantId + Payment.CustomerSessionId
            // phải cùng Restaurant với CustomerSession.
            builder.HasOne(x => x.CustomerSession)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.CustomerSessionId
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
