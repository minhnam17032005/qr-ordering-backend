using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Platform
{
    public class ServiceRegistration : BaseEntity
    {
        // Thông tin người đăng ký
        public string ContactName { get; set; } = null!;

        public string Email { get; set; } = null!;
        
        public string? PhoneNumber { get; set; }

        // Thông tin nhà hàng
        public string RestaurantName { get; set; } = null!;

        public string? RestaurantAddress { get; set; }


        // Trạng thái đăng ký
        public ServiceRegistrationStatus Status { get; set; } = ServiceRegistrationStatus.Pending;

        public string? Note { get; set; }

        // Người Platform xử lý

        public Guid? ProcessedByPlatformAdminId { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public Guid? RestaurantId { get; set; }

        // Navigation Properties
        public PlatformAdmin? ProcessedByPlatformAdmin { get; set; }

        public Restaurant? Restaurant { get; set; }
    }
}
