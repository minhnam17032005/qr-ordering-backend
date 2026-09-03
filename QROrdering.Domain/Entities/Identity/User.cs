using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Domain.Entities.Identity
{
    public class User : BaseEntity
    {

        public string FullName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } 

        // Navigation Properties

        public ICollection<UserSession> UserSessions { get; set; }
            = new List<UserSession>();

        public ICollection<RestaurantMember> RestaurantMembers { get; set; }
            = new List<RestaurantMember>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
