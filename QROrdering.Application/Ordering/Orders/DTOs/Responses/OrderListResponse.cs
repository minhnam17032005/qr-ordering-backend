using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Responses
{
    public class OrderListResponse
    {
        public Guid Id { get; set; }

        public string OrderCode { get; set; } = null!;

        public int TableNumber { get; set; }

        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }

        public int ItemCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}