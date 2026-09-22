using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Users.DTOs.Responses
{
    public class PlatformUserListResponse
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool IsActive { get; set; }

        public List<PlatformUserRestaurantResponse> Restaurants { get; set; }
            = new();
    }
}
