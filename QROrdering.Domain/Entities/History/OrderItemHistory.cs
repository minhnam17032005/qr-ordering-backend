using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.History
{
    public class OrderItemHistory : BaseEntity
    {
        public Guid OrderHistoryId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public OrderItemStatus Status { get; set; }

        // Navigation Properties

        public OrderHistory OrderHistory { get; set; } = null!;
    }
}
