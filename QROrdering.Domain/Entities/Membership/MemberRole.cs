using QROrdering.Domain.Entities.Authorization;

namespace QROrdering.Domain.Entities.Membership
{
    public class MemberRole : BaseEntity
    {
        public Guid RestaurantId { get; set; }
        public Guid RestaurantMemberId { get; set; }

        public Guid RoleId { get; set; }

        // Navigation Properties
        public RestaurantMember RestaurantMember { get; set; }
            = null!;

        public Role Role { get; set; } = null!;
    }
}
