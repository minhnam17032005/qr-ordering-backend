using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Entities.Ordering;

namespace QROrdering.Domain.Entities.Identity
{
    public class User : BaseEntity
    {

        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        // nên thêm EmailVerified khi muốn yêu cầu đăng ký khắt khe hơn
        public string PasswordHash { get; set; } = null!;

        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties

        public ICollection<UserSession> UserSessions { get; set; }
            = new List<UserSession>();
        public ICollection<RestaurantMember> RestaurantMembers { get; set; }
            = new List<RestaurantMember>();
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
