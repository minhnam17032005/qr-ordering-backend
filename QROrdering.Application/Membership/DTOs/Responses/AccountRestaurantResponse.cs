using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class AccountRestaurantResponse
    {
        public Guid RestaurantId { get; set; }

        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public List<AccountRestaurantRoleResponse> Roles { get; set; }
            = new();
    }
}
