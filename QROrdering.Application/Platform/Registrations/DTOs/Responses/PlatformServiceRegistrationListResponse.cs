using QROrdering.Domain.Enums;

namespace QROrdering.Application.Platform.Registrations.DTOs.Responses
{
    public class PlatformServiceRegistrationListResponse
    {
        public Guid Id { get; set; }

        public string ContactName { get; set; } = null!;

        public string RestaurantName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}