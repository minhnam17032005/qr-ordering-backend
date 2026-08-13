using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid CustomerSessionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; } = null!;

        [Required]
        public OrderStatus Status { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public CustomerSession CustomerSession { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
