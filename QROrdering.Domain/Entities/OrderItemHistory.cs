using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class OrderItemHistory : BaseEntity
    {
        public Guid OrderHistoryId { get; set; }

        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        public OrderItemStatus Status { get; set; }

        // Navigation Properties

        public OrderHistory OrderHistory { get; set; } = null!;
    }
}
