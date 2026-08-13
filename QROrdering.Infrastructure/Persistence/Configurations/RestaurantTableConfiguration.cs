using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;
using QROrdering.Domain.Enums;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: RestaurantTables
    public class RestaurantTableConfiguration : IEntityTypeConfiguration<RestaurantTable>
    {
        public void Configure(EntityTypeBuilder<RestaurantTable> builder)
        {
            builder.ToTable("RestaurantTables");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();

            // =========================
            // Relationships
            // =========================

            // Restaurant 1 - N RestaurantTable
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.RestaurantTables)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // RestaurantTable 1 - N CustomerSession
            builder.HasMany(x => x.CustomerSessions)
                .WithOne(x => x.RestaurantTable)
                .HasForeignKey(x => x.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // TableNumber must be unique within the same Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.TableNumber
            })
            .IsUnique();

            // QRCode must be globally unique
            builder.HasIndex(x => x.QRCode)
                .IsUnique();

        }
    }
}