using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.DTOs
{
    public class RestaurantMembershipResponse
    {
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = null!;

        public List<MemberRoleResponse> Roles { get; set; }
            = new();
    }
}
