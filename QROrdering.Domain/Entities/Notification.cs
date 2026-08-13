using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = null!;

        [Required]
        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public User? User { get; set; }

        public Order? Order { get; set; }
    }
}
