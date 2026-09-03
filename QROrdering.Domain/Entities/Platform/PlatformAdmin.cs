using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities.Platform
{
    public class PlatformAdmin : BaseEntity
    {
        public string FullName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } 

        // Navigation Properties

        public ICollection<PlatformAdminSession> Sessions { get; set; }
            = new List<PlatformAdminSession>();

        public ICollection<ServiceRegistration> ServiceRegistrations { get; set; }
            = new List<ServiceRegistration>();
    }
}