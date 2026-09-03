using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Domain.Entities.Authorization
{
    public class Role : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } 

        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<MemberRole> MemberRoles { get; set; }
            = new List<MemberRole>();

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
