using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.Ordering
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal Amount { get; set; }

        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation Properties

        public Order Order { get; set; } = null!;
    }
}
