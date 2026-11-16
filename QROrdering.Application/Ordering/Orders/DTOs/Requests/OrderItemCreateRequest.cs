using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.Ordering.Orders.DTOs.Requests
{
    public class OrderItemCreateRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}