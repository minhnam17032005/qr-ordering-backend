using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.History
{
    public class OrderHistory : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = null!;

        public int TableNumber { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<OrderItemHistory> OrderItemHistories { get; set; }
            = new List<OrderItemHistory>();
    }
}
