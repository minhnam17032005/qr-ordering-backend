using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Required]
        public OrderItemStatus Status { get; set; }

        // Navigation Properties

        public Order Order { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
