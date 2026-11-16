using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Requests
{
    public class OrderStatusUpdateRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}