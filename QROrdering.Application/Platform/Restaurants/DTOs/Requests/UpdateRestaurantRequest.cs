using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Restaurants.DTOs.Requests
{
    public class UpdateRestaurantRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? LogoUrl { get; set; }
    }
}
