using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Responses
{
    public class OrderItemStatusResponse
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public OrderItemStatus Status { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}