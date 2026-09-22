using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Users.DTOs.Responses
{
    public class PlatformUserRestaurantResponse
    {
        public Guid RestaurantId { get; set; }

        public string RestaurantName { get; set; } = null!;

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new();
    }

}
