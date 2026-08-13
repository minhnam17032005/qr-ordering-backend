using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    // Cấu hình bảng: Categories
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.ToTable("Categories");

            // BaseEntity: Id, CreatedAt, UpdatedAt
            builder.ConfigureBaseEntity();


            // =========================
            // Relationships
            // =========================

            // Restaurant 1 - N Category
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category 1 - N Product
            builder.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Index / Unique
            // =========================

            // Category name must be unique within the same Restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.Name
            })
            .IsUnique();


        }
    }
}