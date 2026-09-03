using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Domain.Entities.Membership
{
    public class RestaurantMember : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid RestaurantId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public User User { get; set; } = null!;

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<MemberRole> MemberRoles { get; set; }
            = new List<MemberRole>();
    }
}
