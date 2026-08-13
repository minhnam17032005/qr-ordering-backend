using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: CustomerSessions
    public class CustomerSessionConfiguration : IEntityTypeConfiguration<CustomerSession>
    {
        public void Configure(EntityTypeBuilder<CustomerSession> builder)
        {
            builder.ToTable("CustomerSessions");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // =========================
            // Relationships
            // =========================

            // Restaurant 1 - N CustomerSession
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.CustomerSessions)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // RestaurantTable 1 - N CustomerSession
            builder.HasOne(x => x.RestaurantTable)
                .WithMany(x => x.CustomerSessions)
                .HasForeignKey(x => x.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // CustomerSession 1 - N Order
            builder.HasMany(x => x.Orders)
                .WithOne(x => x.CustomerSession)
                .HasForeignKey(x => x.CustomerSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // SessionToken identifies a session
            builder.HasIndex(x => x.SessionToken)
                .IsUnique();

            // Query sessions by Restaurant + Table
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.TableId
            });

            // Query active/current sessions
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.TableId,
                x.Status
            });
        }
    }
}