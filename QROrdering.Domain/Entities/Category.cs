using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities
{
    public class Category : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Url]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
