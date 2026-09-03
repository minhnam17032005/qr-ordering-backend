using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities.Platform
{
    public class PlatformAdminSession : BaseEntity
    {
        public Guid PlatformAdminId { get; set; }

        public string RefreshTokenHash { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime LastActivityAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        // Navigation Properties

        public PlatformAdmin PlatformAdmin { get; set; } = null!;
    }
}