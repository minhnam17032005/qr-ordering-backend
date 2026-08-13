using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(500)]
        [Url]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public Category Category { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
