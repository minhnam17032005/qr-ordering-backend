using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Ordering
{
    public class CustomerSession : BaseEntity
    {
        public Guid RestaurantId { get; set; }
        public Guid TableId { get; set; }

        public string? CustomerName { get; set; }

        public string SessionToken { get; set; } = null!;

        public CustomerSessionStatus Status { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public RestaurantTable RestaurantTable { get; set; } = null!;

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}