using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class AddMemberResponse
    {
        public Guid MemberId { get; set; }

        public Guid UserId { get; set; }

        public Guid RestaurantId { get; set; }

        public bool IsActive { get; set; }

        public List<AssignedRoleResponse> Roles { get; set; } = new();
    }
}
