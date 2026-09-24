using QROrdering.Domain.Enums;

namespace QROrdering.Application.Platform.Registrations.DTOs.Responses
{
    public class PlatformServiceRegistrationDetailResponse
    {
        public Guid Id { get; set; }

        // Người đăng ký

        public Guid UserId { get; set; }

        public string ContactName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }


        // Thông tin nhà hàng

        public string RestaurantName { get; set; } = null!;

        public string RestaurantAddress { get; set; } = null!;

        public string? RestaurantPhoneNumber { get; set; }

        public string? RestaurantEmail { get; set; }

        public string? RestaurantDescription { get; set; }

        public string? RestaurantLogoUrl { get; set; }


        // Trạng thái

        public string Status { get; set; } = null!;

        public string? Note { get; set; }


        // Platform xử lý

        public Guid? ProcessedByPlatformAdminId { get; set; }

        public DateTime? ProcessedAt { get; set; }


        // Restaurant sau khi approve

        public Guid? RestaurantId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}