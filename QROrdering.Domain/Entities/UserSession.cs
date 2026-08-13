using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities
{
    public class UserSession : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string RefreshTokenHash { get; set; } = null!;

        [MaxLength(100)]
        public string? DeviceName { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(1000)]
        public string? UserAgent { get; set; }

        [Required]
        public DateTime ExpiredAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        [Required]
        public DateTime LastAccessAt { get; set; }

        // Navigation Properties

        public User User { get; set; } = null!;
    }
}