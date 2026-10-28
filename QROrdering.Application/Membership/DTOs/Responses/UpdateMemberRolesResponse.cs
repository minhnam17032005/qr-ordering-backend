using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class UpdateMemberRolesResponse
    {
        public Guid MemberId { get; set; }
        public Guid RestaurantId { get; set; }
        public List<AssignedRoleResponse> Roles { get; set; } = new();
    }
}
