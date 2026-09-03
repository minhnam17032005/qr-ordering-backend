using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Authorization
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string ApiPath { get; set; } = null!;

        public PermissionMethod Method { get; set; }

        public string Module { get; set; } = null!;

        // Navigation Properties

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
