using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class CustomerSessionConfiguration
        : IEntityTypeConfiguration<CustomerSession>
    {
        public void Configure(EntityTypeBuilder<CustomerSession> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("CustomerSessions");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.CustomerName)
                .HasMaxLength(100);

            builder.Property(x => x.SessionToken)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.StartedAt)
                .IsRequired();

            builder.Property(x => x.EndedAt)
                .IsRequired(false);

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

            builder.HasIndex(x => x.SessionToken)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.TableId,
                x.Status
            });


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N CustomerSession
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.CustomerSessions)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);


            // RestaurantTable 1 - N CustomerSession
            // Cross-Tenant Protection:
            // CustomerSession.RestaurantId + CustomerSession.TableId
            // phải cùng Restaurant với RestaurantTable.
            builder.HasOne(x => x.RestaurantTable)
                .WithMany(x => x.CustomerSessions)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.TableId
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