using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Responses
{
    public class OrderStatusResponse
    {
        public Guid Id { get; set; }

        public OrderStatus Status { get; set; }
    }
}