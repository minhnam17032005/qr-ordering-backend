using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class AccountRestaurantRoleResponse
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; } = null!;

        public int Type { get; set; }
    }
}
