using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Domain.Entities.Ordering
{
    public class RevenueDaily : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public DateTime RevenueDate { get; set; }

        public int TotalOrders { get; set; }

        public int CompletedOrders { get; set; }

        public int CancelledOrders { get; set; }

        public int TotalItemsSold { get; set; }

        public decimal TotalRevenue { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;
    }
}
