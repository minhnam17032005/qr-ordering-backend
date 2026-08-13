using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        [Url]
        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<UserSession> UserSessions { get; set; }
            = new List<UserSession>();

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
