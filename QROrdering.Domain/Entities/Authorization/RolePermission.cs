namespace QROrdering.Domain.Entities.Authorization
{
    public class RolePermission
    {

        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        // Navigation Properties

        public Role Role { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }
}
