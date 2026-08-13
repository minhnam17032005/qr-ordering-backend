using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: Products
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();


            // Relationship: Restaurant 1 - N Product
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Category 1 - N Product
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Product 1 - N OrderItem
            builder.HasMany(x => x.OrderItems)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // Product name must be unique within the same Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Name
            })
            .IsUnique();

            // Query products by Restaurant + Category
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.CategoryId
            });

        }
    }
}