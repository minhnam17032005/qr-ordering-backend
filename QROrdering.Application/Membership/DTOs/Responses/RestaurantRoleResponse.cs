using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Membership.DTOs.Responses
{
    public class RestaurantRoleResponse
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
    }
}
