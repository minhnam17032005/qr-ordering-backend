using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities
{
    public class RevenueDaily : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        [Required]
        public DateTime RevenueDate { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalOrders { get; set; }

        [Range(0, int.MaxValue)]
        public int CompletedOrders { get; set; }

        [Range(0, int.MaxValue)]
        public int CancelledOrders { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalItemsSold { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalRevenue { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;
    }
}
