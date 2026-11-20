using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Ordering
{
    public class Payment : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public Guid CustomerSessionId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal Amount { get; set; }

        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation Properties
        public Restaurant Restaurant { get; set; } = null!;

        public CustomerSession CustomerSession { get; set; } = null!;
    }
}