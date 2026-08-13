using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class OrderHistory : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int TableNumber { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<OrderItemHistory> OrderItemHistories { get; set; }
            = new List<OrderItemHistory>();
    }
}
