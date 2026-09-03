using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities.RestaurantManagement
{
    public class Category : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } 

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}