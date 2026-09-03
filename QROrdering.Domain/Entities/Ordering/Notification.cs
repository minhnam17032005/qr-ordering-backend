using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Ordering
{
    public class Notification : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public User? User { get; set; }

        public Order? Order { get; set; }
    }
}
