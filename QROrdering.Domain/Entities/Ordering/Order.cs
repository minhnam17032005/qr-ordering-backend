using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Ordering
{
    public class Order : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid CustomerSessionId { get; set; }

        public string OrderCode { get; set; } = null!;

        public OrderStatus Status { get; set; }

        public string? Note { get; set; }

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