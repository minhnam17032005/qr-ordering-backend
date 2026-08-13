using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class CustomerSession : BaseEntity
    {
        public Guid RestaurantId { get; set; }
        public Guid TableId { get; set; }

        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [Required]
        [MaxLength(100)]
        public string SessionToken { get; set; } = null!;

        [Required]
        public CustomerSessionStatus Status { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public RestaurantTable RestaurantTable { get; set; } = null!;

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}
