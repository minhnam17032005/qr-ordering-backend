using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Ordering
{
    public class OrderItem : BaseEntity
    {
        public Guid RestaurantId { get; set; }
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string? Note { get; set; }

        public OrderItemStatus Status { get; set; }

        // Navigation Properties

        public Order Order { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
