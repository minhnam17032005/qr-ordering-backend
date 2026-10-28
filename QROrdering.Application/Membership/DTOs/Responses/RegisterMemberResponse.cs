using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class RegisterMemberResponse
    {
        public Guid UserId { get; set; }
        public Guid MemberId { get; set; }
        public Guid RestaurantId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<AssignedRoleResponse> Roles { get; set; } = new();
    }
}
