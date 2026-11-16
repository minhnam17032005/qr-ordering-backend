using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.DTOs.Requests
{
    public class OrderItemStatusUpdateRequest
    {
        [Required]
        public OrderItemStatus Status { get; set; }
    }
}