using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration
        : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("Products");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

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

            // Tìm Product theo Restaurant
            builder.HasIndex(x => x.RestaurantId);

            // Tìm Product theo Category
            builder.HasIndex(x => x.CategoryId);

            // Product name unique trong phạm vi Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Name
            })
            .IsUnique();


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // Restaurant 1 - N Product
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);


            // Category 1 - N Product
            // Cross-Tenant Protection:
            // Product.RestaurantId + Product.CategoryId
            // phải cùng Restaurant với Category.
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => new
                {
                    x.RestaurantId,
                    x.CategoryId
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