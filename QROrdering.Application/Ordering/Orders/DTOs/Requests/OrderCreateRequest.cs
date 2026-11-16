using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.Ordering.Orders.DTOs.Requests
{
    public class OrderCreateRequest
    {
        [MaxLength(500)]
        public string? Note { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một sản phẩm.")]
        public List<OrderItemCreateRequest> Items { get; set; } = [];
    }
}