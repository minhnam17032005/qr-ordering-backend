using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Responses
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public OrderItemStatus Status { get; set; }
    }
}