using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Domain.Entities.RestaurantManagement
{
    public class Product : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } 

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public Category Category { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
