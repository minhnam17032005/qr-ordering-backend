using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Responses
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public Guid CustomerSessionId { get; set; }

        public string OrderCode { get; set; } = null!;

        public int TableNumber { get; set; }

        public OrderStatus Status { get; set; }

        public string? Note { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<OrderItemResponse> Items { get; set; } = [];
    }
}